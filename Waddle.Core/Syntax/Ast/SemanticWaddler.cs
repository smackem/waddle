using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Waddle.Core.Symbols;

namespace Waddle.Core.Syntax.Ast
{
    public class SemanticWaddler
    {
        private readonly IImmutableDictionary<string, FunctionDecl> _functions;
        private FunctionDecl _currentFunction;

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
                    hasEntryPoint = true;
                }
                WaddleFunctionDeclaration(function);
            }

            if (hasEntryPoint == false)
            {
                throw new SemanticErrorException("Program has no entry Point.");
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
                BoolLiteralAtom boolAtom => TypeSymbol.Bool,
                IntegerLiteralAtom integerAtom => TypeSymbol.Integer,
                StringLiteralAtom stringAtom => TypeSymbol.String,
                IdentifierAtom identifierAtom => _currentFunction.Variables[identifierAtom.Identifier].Type ?? TypeSymbol.Null,
                _ => throw new ArgumentOutOfRangeException(nameof(exprStmt))
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
            return _functions[invocationExpr.Identifier].Type ?? TypeSymbol.Null;
        }

        private static bool IsAssignableFrom(TypeSymbol left, TypeSymbol right)
        {
            return left == right;
        }
    }
}