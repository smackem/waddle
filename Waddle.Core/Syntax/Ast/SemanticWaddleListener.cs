using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Waddle.Core.Syntax.Ast
{
    public class SemanticWaddleListener : IWaddleListener
    {
        private readonly IImmutableDictionary<string, FunctionDecl> _functions;
        private FunctionDecl? _currentFunction;

        private TypeExtractingVisitor TypeVisitor
        {
            get
            {
                var visitorDict = new Dictionary<string, Symbol>();
                foreach (var (s, functionDecl) in _functions)
                {
                    visitorDict.Add(s, functionDecl);
                }
                
                if(_currentFunction != null)
                {
                    foreach (var (key, value) in _currentFunction.Variables)
                    {
                        visitorDict.Add(key, value);
                    }
                }

                return new TypeExtractingVisitor(visitorDict);
            }
        }
        
        public SemanticWaddleListener(IImmutableDictionary<string, FunctionDecl> functions)
        {
            _functions = functions;
        }
        
        public bool EnterProgram(ProgramSyntax syntax, WaddleContext ctx)
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
            
            return true;
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

        public void LeaveProgram(ProgramSyntax syntax, WaddleContext ctx)
        {
            
        }

        public bool EnterFunctionDeclaration(FunctionDeclSyntax syntax, WaddleContext ctx)
        {
            _currentFunction = _currentFunction = _functions[syntax.Name];
            return true;
        }

        public void LeaveFunctionDeclaration(FunctionDeclSyntax syntax, WaddleContext ctx)
        {
            
        }

        public bool EnterPrintStmt(PrintStmtSyntax syntax, WaddleContext ctx)
        {
            foreach (var printStmtArgument in syntax.Arguments)
            {
                var exprType = printStmtArgument.Accept(TypeVisitor);
                if (exprType == TypeSymbol.Void)
                {
                    throw new SemanticErrorException("Can not print void-value.");
                }
            }

            return true;
        }

        public void LeavePrintStmt(PrintStmtSyntax syntax, WaddleContext ctx)
        {
            
        }

        public bool EnterIdentifierLiteral(IdentifierAtom atom, WaddleContext ctx)
        {
            var _ = _currentFunction?.Variables[atom.Identifier].Type
                ?? throw new SemanticErrorException($"{atom.Identifier} does not have a type");
            return true;
        }

        public void LeaveIdentifierLiteral(IdentifierAtom atom, WaddleContext ctx)
        {
            
        }

        public bool EnterBoolLiteral(BoolLiteralAtom atom, WaddleContext ctx)
        {
            return true;
        }

        public void LeaveBoolLiteral(BoolLiteralAtom atom, WaddleContext ctx)
        {
            
        }

        public bool EnterIntegerLiteral(IntegerLiteralAtom atom, WaddleContext ctx)
        {
            return true;
        }

        public void LeaveIntegerLiteral(IntegerLiteralAtom atom, WaddleContext ctx)
        {
            
        }

        public bool EnterStringLiteral(StringLiteralAtom atom, WaddleContext ctx)
        {
            return true;
        }

        public void LeaveStringLiteral(StringLiteralAtom atom, WaddleContext ctx)
        {
            
        }

        public bool EnterTermExpr(TermExpressionSyntax termExpr, WaddleContext ctx)
        {
            if (termExpr.Left.Accept(TypeVisitor) != termExpr.Right.Accept(TypeVisitor))
            {
                throw new SemanticErrorException("incompatible types");
            }

            return true;
        }

        public void LeaveTermExpr(TermExpressionSyntax termExpr, WaddleContext ctx)
        {
            
        }

        public bool EnterProductExpr(ProductExpressionSyntax productExpr, WaddleContext ctx)
        {
            if (productExpr.Left.Accept(TypeVisitor) != productExpr.Right.Accept(TypeVisitor))
            {
                throw new SemanticErrorException("incompatible types");
            }

            return true;
        }

        public void LeaveProductExpr(ProductExpressionSyntax productExpr, WaddleContext ctx)
        {
            
        }

        public bool EnterRelationalExpr(RelationalExpressionSyntax relationalExpr, WaddleContext ctx)
        {
            if (relationalExpr.Left.Accept(TypeVisitor) != relationalExpr.Right.Accept(TypeVisitor))
            {
                throw new SemanticErrorException("incompatible types");
            }

            return true;
        }

        public void LeaveRelationalExpr(RelationalExpressionSyntax relationalExpr, WaddleContext ctx)
        {
            
        }

        public bool EnterLogicalExpr(LogicalExpressionSyntax logicalExpr, WaddleContext ctx)
        {
            if (logicalExpr.Left.Accept(TypeVisitor) != logicalExpr.Right.Accept(TypeVisitor))
            {
                throw new SemanticErrorException("incompatible types");
            }

            return true;
        }

        public void LeaveLogicalExpr(LogicalExpressionSyntax logicalExpr, WaddleContext ctx)
        {
            
        }

        public bool EnterInvocationExpr(InvocationExpressionSyntax invocationExpr, WaddleContext ctx)
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
                var exprType = arguments[i].Accept(TypeVisitor);
                if (parameter.Type != exprType)
                {
                    throw new SemanticErrorException($"Parameter {parameter} requires type {parameter.Type}, but type {exprType} was given.");
                }
            }
            
            return true;
        }

        public void LeaveInvocationExpr(InvocationExpressionSyntax invocationExpr, WaddleContext ctx)
        {
            
        }

        public bool EnterIfStmt(IfStmtSyntax ifStmt, WaddleContext ctx)
        {
            if (_currentFunction == null)
            {
                throw new SemanticErrorException($"can not use if statement outside of function.");
            }

            // check that expr is boolean
            var exprType = ifStmt.Expression.Accept(TypeVisitor);
            if (TypeSymbol.Bool != exprType)
            {
                throw new SemanticErrorException("If-Statement expression must result in boolean value.");
            }

            return true;
        }

        public void LeaveIfStmt(IfStmtSyntax ifStmt, WaddleContext ctx)
        {
            
        }

        public bool EnterReturnStmt(ReturnStmtSyntax returnStmt, WaddleContext ctx)
        {
            
            if (_currentFunction == null)
            {
                throw new SemanticErrorException($"can not use return statement outside of function.");
            }

            if (_currentFunction.Type == null)
            {
                //Ignore Type if function is typeless
                return true;
            }

            // check IsAssignableFrom (function.type, expr.type)
            var exprType = returnStmt.Expression.Accept(TypeVisitor);
            if (_currentFunction.Type != exprType)
            {
                throw new SemanticErrorException(
                    $"Function result is of type {_currentFunction.Type!}, the type {exprType} can not be assigned as result.");
            }
            
            return true;
        }

        public void LeaveReturnStmt(ReturnStmtSyntax returnStmt, WaddleContext ctx)
        {
            
        }

        public bool EnterAssignStmt(AssignStmtSyntax assignStmt, WaddleContext ctx)
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
            var exprType = assignStmt.Expression.Accept(TypeVisitor);

            // check IsAssignableFrom
            if (identifier.Type  != exprType)
            {
                throw new SemanticErrorException($"The type {exprType} can not be assigned into {identifier.Name}.");
            }

            return true;
        }

        public void LeaveAssignStmt(AssignStmtSyntax assignStmt, WaddleContext ctx)
        {
            
        }

        public bool EnterDeclStmt(DeclStmtSyntax declStmt, WaddleContext ctx)
        {
            if (_currentFunction == null)
            {
                throw new SemanticErrorException($"can not use declare statement outside of function.");
            }

            var variable = _currentFunction?.Variables[declStmt.ParameterDeclSyntax.Name]!;
            var exprType = declStmt.Expression.Accept(TypeVisitor);

            if (variable.Type != exprType)
            {
                throw new SemanticErrorException($"type {variable.Type} is not assignable from {exprType}.");
            }

            return true;
        }

        public void LeaveDeclStmt(DeclStmtSyntax declStmt, WaddleContext ctx)
        {
            
        }

        public bool EnterBlock(BlockSyntax block, WaddleContext ctx)
        {
            return true;
        }

        public void LeaveBlock(BlockSyntax block, WaddleContext ctx)
        {
            var currentParent = ctx.Parent as FunctionDeclSyntax;
            if (currentParent != null)
            {
                if (currentParent.Name == _currentFunction!.Name && _currentFunction.Type != null)
                {
                    StatementSyntax? lastStmnt = block.Statements.Last();
                    if (lastStmnt is ReturnStmtSyntax == false)
                    {
                        throw new SemanticErrorException("Last Statement must be return if function has a type");
                    }
                }
            }
        }
    }
}