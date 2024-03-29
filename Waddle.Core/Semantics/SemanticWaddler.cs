using System;
using System.Collections.Immutable;
using System.Linq;
using Waddle.Core.Ast;
using Waddle.Core.Syntax;

namespace Waddle.Core.Semantics
{
    public class SemanticWaddler
    {
        private readonly IImmutableDictionary<string, FunctionDecl> _functions;
        private FunctionDecl? _currentFunction;

        public SemanticWaddler(IImmutableDictionary<string, FunctionDecl> functions)
        {
            _functions = functions;
        }

        public void WaddleProgram(ProgramSyntax program)
        {
            var hasEntryPoint = false;

            foreach (var function in program.FunctionDeclarations)
            {
                _currentFunction = _functions[function.Name];

                if (function.Name == Naming.EntryPointFunctionName)
                {
                    ValidateEntryPointFunction(_currentFunction);
                    hasEntryPoint = true;
                }

                WaddleFunctionDeclaration(function);
            }

            if (hasEntryPoint == false)
            {
                throw new SemanticErrorException("Program has no entry Point.");
            }
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

        private void WaddleFunctionDeclaration(FunctionDeclSyntax function)
        {
            WaddleBody(function.Body);

            if (_currentFunction?.Type is not null && function.Body.Statements.LastOrDefault() is not ReturnStmtSyntax)
            {
                throw new SemanticErrorException($"missing return statement in function {_currentFunction?.Name}");
            }
        }

        private void WaddleBody(BlockSyntax blockSyntax)
        {
            foreach (var stmt in blockSyntax.Statements.ToArray())
            {
                WaddleStmt(stmt);
            }
        }

        private void WaddleStmt(StatementSyntax stmt)
        {
            switch (stmt)
            {
                case DeclStmtSyntax declStmt:
                    WaddleDeclStmt(declStmt);
                    break;
                case AssignStmtSyntax assignStmt:
                    WaddleAssignStmt(assignStmt);
                    break;
                case ReturnStmtSyntax returnStmt:
                    WaddleReturnStmt(returnStmt);
                    break;
                case IfStmtSyntax ifStmt:
                    WaddleIfStmt(ifStmt);
                    break;
                case InvocationStmtSyntax invocationStmt:
                    // check inner InvocationExpressionSyntax
                    WaddleExpression(invocationStmt.Expression);
                    break;
                case PrintStmtSyntax printStmt:
                    // check that expr is not void
                    WaddlePrintStmt(printStmt);
                    break;
            }
        }

        private void WaddlePrintStmt(PrintStmtSyntax printStmt)
        {
            foreach (var printStmtArgument in printStmt.Arguments)
            {
                var exprType = WaddleExpression(printStmtArgument);
                if (exprType == TypeSymbol.Void)
                {
                    throw new SemanticErrorException("Can not print void-value.");
                }
            }
        }

        private void WaddleIfStmt(IfStmtSyntax ifStmt)
        {
            if (_currentFunction == null)
            {
                throw new SemanticErrorException($"can not use if statement outside of function.");
            }

            // check that expr is boolean
            var exprType = WaddleExpression(ifStmt.Expression);
            if (IsAssignableFrom(TypeSymbol.Bool, exprType) == false)
            {
                throw new SemanticErrorException("If-Statement expression must result in boolean value.");
            }

            WaddleBody(ifStmt.Body);
        }

        private void WaddleReturnStmt(ReturnStmtSyntax returnStmt)
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

            // check IsAssignableFrom (function.type, expr.type)
            var exprType = WaddleExpression(returnStmt.Expression);
            if (IsAssignableFrom(_currentFunction.Type!, exprType) == false)
            {
                throw new SemanticErrorException(
                    $"Function result is of type {_currentFunction.Type!}, the type {exprType} can not be assigned as result.");
            }
        }

        private void WaddleAssignStmt(AssignStmtSyntax assignStmt)
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
            var exprType = WaddleExpression(assignStmt.Expression);

            // check IsAssignableFrom
            if (IsAssignableFrom(identifier.Type!, exprType) == false)
            {
                throw new SemanticErrorException($"The type {exprType} can not be assigned into {identifier.Name}.");
            }
        }

        private void WaddleDeclStmt(DeclStmtSyntax declStmt)
        {
            if (_currentFunction == null)
            {
                throw new SemanticErrorException($"can not use declare statement outside of function.");
            }

            if (_currentFunction?.Variables.ContainsKey(declStmt.ParameterDeclSyntax.Name) == false)
            {
                throw new SemanticErrorException($"{declStmt.ParameterDeclSyntax.Name} is not an available variable at this position.");
            }
            
            var variable = _currentFunction?.Variables[declStmt.ParameterDeclSyntax.Name]!;
            var exprType = WaddleExpression(declStmt.Expression);

            if (IsAssignableFrom(variable.Type!, exprType) == false)
            {
                throw new SemanticErrorException($"type {variable.Type} is not assignable from {exprType}.");
            }
        }

        private TypeSymbol WaddleExpression(ExpressionSyntax exprStmt)
        {
            return exprStmt switch
            {
                LogicalExpressionSyntax logicalExpr => WaddleLogicalExpr(logicalExpr),
                ProductExpressionSyntax productExpr => WaddleProductExpr(productExpr),
                RelationalExpressionSyntax relationalExpr => WaddleRelationalExpr(relationalExpr),
                TermExpressionSyntax termExpr => WaddleTermExpr(termExpr),
                InvocationExpressionSyntax invocationExpr => WaddleInvocationExpr(invocationExpr),
                BoolLiteralAtom _ => TypeSymbol.Bool,
                IntegerLiteralAtom _ => TypeSymbol.Integer,
                StringLiteralAtom _ => TypeSymbol.String,
                IdentifierAtom identifierAtom => _currentFunction?.Variables[identifierAtom.Identifier].Type
                                                 ?? throw new SemanticErrorException($"{identifierAtom.Identifier} does not have a type"),
                _ => throw new ArgumentOutOfRangeException(nameof(exprStmt)),
            };
        }

        private TypeSymbol WaddleTermExpr(TermExpressionSyntax termExpr)
        {
            if (WaddleExpression(termExpr.Left) != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Number @({termExpr.Left.StartToken.LineNumber}{termExpr.Left.StartToken.CharPosition}).");
            }
            if (WaddleExpression(termExpr.Right) != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Number @({termExpr.Right.StartToken.LineNumber}{termExpr.Right.StartToken.CharPosition}).");
            }
            return TypeSymbol.Integer;
        }

        private TypeSymbol WaddleProductExpr(ProductExpressionSyntax productExpr)
        {
            if (WaddleExpression(productExpr.Left) != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Number @({productExpr.Left.StartToken.LineNumber}{productExpr.Left.StartToken.CharPosition}).");
            }
            if (WaddleExpression(productExpr.Right) != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Number @({productExpr.Right.StartToken.LineNumber}{productExpr.Right.StartToken.CharPosition}).");
            }
            return TypeSymbol.Integer;
        }

        private TypeSymbol WaddleLogicalExpr(LogicalExpressionSyntax logicalExpr)
        {
            if (WaddleExpression(logicalExpr.Left) != TypeSymbol.Bool)
            {
                throw new SemanticErrorException($"Not an Bool @({logicalExpr.Left.StartToken.LineNumber}{logicalExpr.Left.StartToken.CharPosition}).");
            }
            if (WaddleExpression(logicalExpr.Right) != TypeSymbol.Bool)
            {
                throw new SemanticErrorException($"Not an Bool @({logicalExpr.Right.StartToken.LineNumber}{logicalExpr.Right.StartToken.CharPosition}).");
            }
            return TypeSymbol.Bool;
        }

        private TypeSymbol WaddleRelationalExpr(RelationalExpressionSyntax relationalExpr)
        {
            // check relationExpr.Left and relationalExpr.Right
            if (WaddleExpression(relationalExpr.Left) != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Integer @({relationalExpr.Left.StartToken.LineNumber}{relationalExpr.Left.StartToken.CharPosition}).");
            }
            if (WaddleExpression(relationalExpr.Right) != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Integer @({relationalExpr.Right.StartToken.LineNumber}{relationalExpr.Right.StartToken.CharPosition}).");
            }
            
            return TypeSymbol.Bool;
        }

        private TypeSymbol WaddleInvocationExpr(InvocationExpressionSyntax invocationExpr)
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
                var exprType = WaddleExpression(arguments[i]);
                if (IsAssignableFrom(parameter.Type!, exprType) == false)
                {
                    throw new SemanticErrorException($"Parameter {parameter} requires type {parameter.Type}, but type {exprType} was given.");
                }
            }
            
            return _functions[invocationExpr.Identifier].Type ?? TypeSymbol.Void;
        }

        private static bool IsAssignableFrom(TypeSymbol left, TypeSymbol right)
        {
            return left == right;
        }
    }
}