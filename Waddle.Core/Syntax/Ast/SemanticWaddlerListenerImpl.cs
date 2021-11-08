using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Waddle.Core.Syntax.Ast
{
    public class SemanticWaddlerListenerImpl : IWaddleListener<TypeSymbol>
    {
        
        private readonly IImmutableDictionary<string, FunctionDecl> _functions;
        private FunctionDecl? _currentFunction;

        public SemanticWaddlerListenerImpl(IImmutableDictionary<string, FunctionDecl> functions)
        {
            _functions = functions;
        }
        
        public TypeSymbol EnterProgram(ProgramSyntax syntax)
        {
            var hasEntryPoint = false;

            foreach (var function in syntax.FunctionDeclarations)
            {
                if (function.Name == Naming.EntryPointFunctionName)
                {
                    ValidateEntryPointFunction(_functions[function.Name]);
                    hasEntryPoint = true;
                }
            }

            if (hasEntryPoint == false)
            {
                throw new SemanticErrorException("Program has no entry Point.");
            }
            return TypeSymbol.Integer;
        }

        private void ValidateEntryPointFunction(FunctionDecl function)
        {
            if (function.Type != TypeSymbol.Integer && function.Type is not null)
            {
                throw new SemanticErrorException("Main must return either void or int");
            }

            if (function.Parameters.Count != 0)
            {
                throw new SemanticErrorException("Main signature mismatch");
            }
        }
        
        public void EnterFunctionDeclaration(FunctionDeclSyntax syntax)
        {
            var stmts = syntax.Body.Statements.ToArray();
            _currentFunction = _currentFunction = _functions[syntax.Name];
            if (_currentFunction?.Type is not null && stmts.LastOrDefault() is not ReturnStmtSyntax)
            {
                throw new SemanticErrorException($"missing return statement in function {_currentFunction?.Name}");
            }
        }

        public TypeSymbol EnterPrintStmt(PrintStmtSyntax syntax, IEnumerable<TypeSymbol> allArguments)
        {
            foreach (var exprType in allArguments)
            {
                if (exprType == TypeSymbol.Void)
                {
                    throw new SemanticErrorException("Can not print void-value.");
                }
            }
            
            return TypeSymbol.Void;
        }

        public TypeSymbol EnterIdentifierLiteral(IdentifierAtom atom)
        {
            return _currentFunction?.Variables[atom.Identifier].Type
                   ?? throw new SemanticErrorException($"{atom.Identifier} does not have a type");
        }

        public TypeSymbol EnterBoolLiteral(BoolLiteralAtom atom)
        {
            return  TypeSymbol.Bool;
        }

        public TypeSymbol EnterIntegerLiteral(IntegerLiteralAtom atom)
        {
            return TypeSymbol.Integer;
        }

        public TypeSymbol EnterStringLiteral(StringLiteralAtom atom)
        {
            return TypeSymbol.String;
        }

        public TypeSymbol EnterTermExpr(TermExpressionSyntax termExpr, Func<TypeSymbol> waddleExpressionLeft, Func<TypeSymbol> waddleExpressionRight)
        {
            if (waddleExpressionLeft.Invoke() != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Number @({termExpr.Left.StartToken.LineNumber}{termExpr.Left.StartToken.CharPosition}).");
            }
            if (waddleExpressionRight.Invoke() != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Number @({termExpr.Right.StartToken.LineNumber}{termExpr.Right.StartToken.CharPosition}).");
            }
            
            return TypeSymbol.Integer;
        }

        public TypeSymbol EnterProductExpr(ProductExpressionSyntax productExpr, Func<TypeSymbol> waddleExpressionLeft, Func<TypeSymbol> waddleExpressionRight)
        {
            if (waddleExpressionLeft.Invoke() != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Number @({productExpr.Left.StartToken.LineNumber}{productExpr.Left.StartToken.CharPosition}).");
            }
            if (waddleExpressionRight.Invoke() != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Number @({productExpr.Right.StartToken.LineNumber}{productExpr.Right.StartToken.CharPosition}).");
            }
            return TypeSymbol.Integer;
        }

        public TypeSymbol EnterRelationalExpr(RelationalExpressionSyntax relationalExpr, Func<TypeSymbol> waddleExpressionLeft,
            Func<TypeSymbol> waddleExpressionRight)
        {
            // check relationExpr.Left and relationalExpr.Right
            if (waddleExpressionLeft.Invoke() != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Integer @({relationalExpr.Left.StartToken.LineNumber}{relationalExpr.Left.StartToken.CharPosition}).");
            }
            if (waddleExpressionRight.Invoke() != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Integer @({relationalExpr.Right.StartToken.LineNumber}{relationalExpr.Right.StartToken.CharPosition}).");
            }
            
            return TypeSymbol.Bool;
        }

        public TypeSymbol EnterLogicalExpr(LogicalExpressionSyntax logicalExpr, Func<TypeSymbol> waddleExpressionLeft, Func<TypeSymbol> waddleExpressionRight)
        {
            if (waddleExpressionLeft.Invoke() != TypeSymbol.Bool)
            {
                throw new SemanticErrorException($"Not an Bool @({logicalExpr.Left.StartToken.LineNumber}{logicalExpr.Left.StartToken.CharPosition}).");
            }
            if (waddleExpressionRight.Invoke() != TypeSymbol.Bool)
            {
                throw new SemanticErrorException($"Not an Bool @({logicalExpr.Right.StartToken.LineNumber}{logicalExpr.Right.StartToken.CharPosition}).");
            }
            return TypeSymbol.Bool;
        }

        public TypeSymbol EnterInvocationExpr(InvocationExpressionSyntax invocationExpr, Func<ExpressionSyntax, TypeSymbol> waddleExpression)
        {
            
            if (_functions.ContainsKey(invocationExpr.Identifier) == false)
            {
                throw new SemanticErrorException($"function {invocationExpr.Identifier} is not defined");
            }

            FunctionDecl functionDecl = _functions[invocationExpr.Identifier]!;
            var parameters = functionDecl.Parameters;
            var arguments = invocationExpr.Arguments.ToArray();

            if (parameters.Count != arguments.Length)
            {
                throw new SemanticErrorException($"function {invocationExpr.Identifier} has {functionDecl.Parameters.Count} parameters, {invocationExpr.Arguments.Count()} arguments where given.");
            }

            // check that for all parameters: IsAssignableFrom(param.Type, arg.Type)
            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                var exprType = waddleExpression(arguments[i]);
                if (IsAssignableFrom(parameter.Type!, exprType) == false)
                {
                    throw new SemanticErrorException($"Parameter {parameter} requires type {parameter.Type}, but type {exprType} was given.");
                }
            }
            
            return _functions[invocationExpr.Identifier].Type ?? TypeSymbol.Void;
        }

        public void EnterIfStmt(IfStmtSyntax ifStmt, TypeSymbol exprType)
        {
            
            if (_currentFunction == null)
            {
                throw new SemanticErrorException($"can not use if statement outside of function.");
            }

            // check that expr is boolean
            if (IsAssignableFrom(TypeSymbol.Bool, exprType) == false)
            {
                throw new SemanticErrorException("If-Statement expression must result in boolean value.");
            }
        }

        public void EnterReturnStmt(ReturnStmtSyntax returnStmt, TypeSymbol returnExprType)
        {
            if (_currentFunction == null)
            {
                throw new SemanticErrorException($"can not use return statement outside of function.");
            }

            if (_currentFunction.Type == null)
            {
                //Ignore Type if function is typeless
                return;
            }
            
            if (IsAssignableFrom(_currentFunction.Type!, returnExprType) == false)
            {
                throw new SemanticErrorException(
                    $"Function result is of type {_currentFunction.Type!}, the type {returnExprType} can not be assigned as result.");
            }
        }

        public void EnterAssignStmt(AssignStmtSyntax assignStmt, TypeSymbol waddleExpression)
        {
            if (_currentFunction == null)
            {
                throw new SemanticErrorException($"can not use assign statement outside of function.");
            }

            if (_currentFunction!.Variables.ContainsKey(assignStmt.StartToken.Lexeme) == false)
            {
                throw new SemanticErrorException(
                    $"unknown identifier ({assignStmt.StartToken.Lexeme}) @({assignStmt.StartToken.LineNumber}{assignStmt.StartToken.CharPosition}).");
            }

            var identifier = _currentFunction?.Variables[assignStmt.StartToken.Lexeme]!;

            // check IsAssignableFrom
            if (IsAssignableFrom(identifier.Type!, waddleExpression) == false)
            {
                throw new SemanticErrorException($"The type {waddleExpression} can not be assigned into {identifier.Name}.");
            }
        }

        public void EnterDeclStmt(DeclStmtSyntax declStmt, TypeSymbol exprType)
        {
            if (_currentFunction == null)
            {
                throw new SemanticErrorException($"can not use declare statement outside of function.");
            }

            var variable = _currentFunction?.Variables[declStmt.ParameterDeclSyntax.Name]!;

            if (IsAssignableFrom(variable.Type!, exprType) == false)
            {
                throw new SemanticErrorException($"type {variable.Type} is not assignable from {exprType}.");
            }
        }
        
        private static bool IsAssignableFrom(TypeSymbol left, TypeSymbol right)
        {
            return left == right;
        }
    }
}