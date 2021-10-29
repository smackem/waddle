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
                    var variable = _currentFunction?.Variables[declStmt.ParameterDeclSyntax.Name]!;
                    var exprType = WaddleExpression(declStmt.Expression);
                    if (IsAssignableFrom(variable.Type!, exprType) == false)
                    {
                        throw new SemanticErrorException($"type {variable.Type} is not assignable from {exprType}");
                    }
                    break;
                case AssignStmtSyntax assignStmt:
                    // check IsAssignableFrom
                    break;
                case ReturnStmtSyntax returnStmt:
                    // check IsAssignableFrom (function.type, expr.type)
                    break;
                case IfStmtSyntax ifStmt:
                    // check that expr is boolean
                    break;
                case InvocationStmtSyntax invocationStmt:
                    // check inner InvocationExpressionSyntax 
                    break;
                case PrintStmtSyntax printStmt:
                    // check that expr is not void 
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
                BoolLiteralAtom _ => TypeSymbol.Bool,
                IntegerLiteralAtom _ => TypeSymbol.Integer,
                StringLiteralAtom _ => TypeSymbol.String,
                IdentifierAtom identifierAtom => _currentFunction?.Variables[identifierAtom.Identifier].Type
                                                 ?? throw new SemanticErrorException($"{identifierAtom.Identifier} does not have a type"),
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
            // check that for all parameters: IsAssignableFrom(param.Type, arg.Type)
            if (_functions.ContainsKey(invocationExpr.Identifier) == false)
            {
                throw new SemanticErrorException($"function {invocationExpr.Identifier} is not defined");
            }
            return _functions[invocationExpr.Identifier].Type ?? TypeSymbol.Void;
        }

        private static bool IsAssignableFrom(TypeSymbol left, TypeSymbol right)
        {
            return left == right;
        }
    }
}