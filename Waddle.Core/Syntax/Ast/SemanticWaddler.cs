using System;
using System.Collections.Immutable;

namespace Waddle.Core.Syntax.Ast
{
    public class SemanticWaddler
    {
        private readonly IImmutableDictionary<string, FunctionDecl> _functions;

        public SemanticWaddler(IImmutableDictionary<string, FunctionDecl> functions)
        {
            _functions = functions;
        }

        public void WaddleProgram(ProgramSyntax program)
        {
            var hasEntryPoint = false;

            foreach (var function in program.FunctionDeclarations)
            {
                if (function.Name == Naming.EntryPointFunctionName)
                {
                    hasEntryPoint = true;
                }
                WaddleFunctionDeclaration(function);
            }

            if (hasEntryPoint == false)
            {
                throw new SemanticErrorException();
            }
        }

        private void WaddleFunctionDeclaration(FunctionDeclSyntax function)
        {
            // find FunctionDecl from symbol table
            foreach (var stmt in function.Body.Statements)
            {
                WaddleStmt(stmt);
            }
        }

        private void WaddleStmt(StatementSyntax stmt)
        {
            switch (stmt)
            {
                case DeclStmtSyntax declStmt:
                    WaddleExpression(declStmt.Expression);
                    break;
                case AssignStmtSyntax assignStmt:
                    break;
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
                IntegerLiteralAtom integerAtom => TypeSymbol.Integer,
                IdentifierAtom identifierAtom => _currentFunction.Variables[identifierAtom.Identifier].Type,
                _ => throw new ArgumentOutOfRangeException(nameof(exprStmt))
            };
        }

        private TypeSymbol WaddleRelationalExpr(RelationalExpressionSyntax relationalExpr)
        {
            // check relationExpr.Left and relationalExpr.Right
            return TypeSymbol.Bool;
        }

        private TypeSymbol WaddleInvocationExpr(InvocationExpressionSyntax invocationExpr)
        {
            return _functions[invocationExpr.Identifier].Type;
        }

        private static bool IsAssignableFrom(TypeSymbol left, TypeSymbol right)
        {
            return left == right;
        }
    }
}