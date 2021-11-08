using System;
using System.Collections.Generic;
using System.Linq;
#if WADDLER
namespace Waddle.Core.Syntax.Ast
{
    public class Waddler<T>
    {
        private readonly IWaddleListener<T> _listener;

        public Waddler(IWaddleListener<T> listener)
        {
            _listener = listener;
        }
        
        public void WaddleProgram(ProgramSyntax program)
        {
            _listener.OnProgram(program);

            foreach (var function in program.FunctionDeclarations)
            {
                WaddleFunctionDeclaration(function);
            }
        }

        private void WaddleFunctionDeclaration(FunctionDeclSyntax function)
        {
            _listener.OnFunctionDeclaration(function);

            var stmts = function.Body.Statements.ToArray();

            foreach (var stmt in stmts)
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
            List <T> arguments = new();
            foreach (var printStmtArgument in printStmt.Arguments)
            {
                arguments.Add(WaddleExpression(printStmtArgument));
            }
            
            _listener.OnPrintStmt(printStmt, arguments);
        }

        private void WaddleIfStmt(IfStmtSyntax ifStmt)
        {
            _listener.OnIfStmt(ifStmt, WaddleExpression(ifStmt.Expression));
            
        }

        private void WaddleReturnStmt(ReturnStmtSyntax returnStmt)
        {
            _listener.OnReturnStmt(returnStmt, WaddleExpression(returnStmt.Expression));
        }

        private void WaddleAssignStmt(AssignStmtSyntax assignStmt)
        {
            _listener.OnAssignStmt(assignStmt, WaddleExpression(assignStmt.Expression));
        }

        private void WaddleDeclStmt(DeclStmtSyntax declStmt)
        {
            _listener.OnDeclStmt(declStmt, WaddleExpression(declStmt.Expression));
        }

        private T WaddleExpression(ExpressionSyntax exprStmt)
        {
            return exprStmt switch
            {
                LogicalExpressionSyntax logicalExpr => WaddleLogicalExpr(logicalExpr),
                ProductExpressionSyntax productExpr => WaddleProductExpr(productExpr),
                RelationalExpressionSyntax relationalExpr => WaddleRelationalExpr(relationalExpr),
                TermExpressionSyntax termExpr => WaddleTermExpr(termExpr),
                InvocationExpressionSyntax invocationExpr => WaddleInvocationExpr(invocationExpr),
                BoolLiteralAtom atom => WaddleBoolLiteral(atom),
                IntegerLiteralAtom atom => WaddleIntegerLiteral(atom),
                StringLiteralAtom atom => WaddleStringLiteral(atom),
                IdentifierAtom atom => WaddleIdentifier(atom),
                _ => throw new ArgumentOutOfRangeException(nameof(exprStmt)),
            };
        }

        private T WaddleIdentifier(IdentifierAtom atom)
        {
            return _listener.OnIdentifierLiteral(atom);
        }

        private T WaddleStringLiteral(StringLiteralAtom atom)
        {
            return _listener.OnStringLiteral(atom);
        }

        private T WaddleIntegerLiteral(IntegerLiteralAtom atom)
        {
            return _listener.OnIntegerLiteral(atom);
        }

        private T WaddleBoolLiteral(BoolLiteralAtom atom)
        {
            return _listener.OnBoolLiteral(atom);
        }

        private T WaddleTermExpr(TermExpressionSyntax termExpr)
        {
            return _listener.OnTermExpr(termExpr, () => WaddleExpression(termExpr.Left), () => WaddleExpression(termExpr.Right));
        }

        private T WaddleProductExpr(ProductExpressionSyntax productExpr)
        {
            return _listener.OnProductExpr(productExpr, () => WaddleExpression(productExpr.Left), () => WaddleExpression(productExpr.Right));
        }

        private T WaddleLogicalExpr(LogicalExpressionSyntax logicalExpr)
        {
            return _listener.OnLogicalExpr(logicalExpr, () => WaddleExpression(logicalExpr.Left), () => WaddleExpression(logicalExpr.Right));
        }

        private T WaddleRelationalExpr(RelationalExpressionSyntax relationalExpr)
        {
            return _listener.OnRelationalExpr(relationalExpr, () => WaddleExpression(relationalExpr.Left), () => WaddleExpression(relationalExpr.Right));
        }

        private T WaddleInvocationExpr(InvocationExpressionSyntax invocationExpr)
        {
            return _listener.OnInvocationExpr(invocationExpr, WaddleExpression);
        }
    }
}
#endif