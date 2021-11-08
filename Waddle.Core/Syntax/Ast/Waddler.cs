using System;
using System.Collections.Generic;
using System.Linq;

namespace Waddle.Core.Syntax.Ast
{
    // TODO: add WaddleContext and stack.Push/Pop calls
    public class Waddler
    {
        private readonly IWaddleListener _listener;
        private readonly Stack<Syntax> _stack = new();

        public Waddler(IWaddleListener listener)
        {
            _listener = listener;
        }

        /// <summary>
        /// Gets a collection containing all ancestors of the current syntax, beginning with the direct parent.
        /// </summary>
        protected IReadOnlyList<Syntax> Ancestors => _stack.ToArray();

        /// <summary>
        /// Gets the direct parent of the current syntax if any.
        /// </summary>
        protected Syntax? Parent => _stack.TryPeek(out var syntax) ? syntax : null;

        public void WaddleProgram(ProgramSyntax program)
        {
            _stack.Push(program);
            _listener.EnterProgram(program);

            foreach (var function in program.FunctionDeclarations)
            {
                WaddleFunctionDeclaration(function);
            }

            _listener.LeaveProgram(program);
            _stack.Pop();
        }

        private void WaddleFunctionDeclaration(FunctionDeclSyntax function)
        {
            _stack.Push(function);
            _listener.EnterFunctionDeclaration(function);

            WaddleBlock(function.Body);

            _listener.LeaveFunctionDeclaration(function);
            _stack.Pop();
        }

        private void WaddleBlock(BlockSyntax functionBody)
        {
            _listener.EnterBlock(functionBody);

            foreach (var stmt in functionBody.Statements)
            {
                WaddleStmt(stmt);
            }
            
            _listener.LeaveBlock(functionBody);
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
            _listener.EnterPrintStmt(printStmt);

            foreach (var printStmtArgument in printStmt.Arguments)
            {
                WaddleExpression(printStmtArgument);
            }

            _listener.LeavePrintStmt(printStmt);
        }

        private void WaddleIfStmt(IfStmtSyntax ifStmt)
        {
            _listener.EnterIfStmt(ifStmt);

            WaddleExpression(ifStmt.Expression);
            WaddleBlock(ifStmt.Body);

            _listener.LeaveIfStmt(ifStmt);
        }

        private void WaddleReturnStmt(ReturnStmtSyntax returnStmt)
        {
            _listener.EnterReturnStmt(returnStmt);

            WaddleExpression(returnStmt.Expression);

            _listener.LeaveReturnStmt(returnStmt);
        }

        private void WaddleAssignStmt(AssignStmtSyntax assignStmt)
        {
            _listener.EnterAssignStmt(assignStmt);

            WaddleExpression(assignStmt.Expression);

            _listener.LeaveAssignStmt(assignStmt);
        }

        private void WaddleDeclStmt(DeclStmtSyntax declStmt)
        {
            _listener.EnterDeclStmt(declStmt);

            WaddleExpression(declStmt.Expression);

            _listener.LeaveDeclStmt(declStmt);
        }

        private void WaddleExpression(ExpressionSyntax exprStmt)
        {
            switch (exprStmt)
            {
                case LogicalExpressionSyntax logicalExpr:
                    WaddleLogicalExpr(logicalExpr);
                    break;
                case ProductExpressionSyntax productExpr:
                    WaddleProductExpr(productExpr);
                    break;
                case RelationalExpressionSyntax relationalExpr:
                    WaddleRelationalExpr(relationalExpr);
                    break;
                case TermExpressionSyntax termExpr:
                    WaddleTermExpr(termExpr);
                    break;
                case InvocationExpressionSyntax invocationExpr:
                    WaddleInvocationExpr(invocationExpr);
                    break;
                case BoolLiteralAtom atom:
                    WaddleBoolLiteral(atom);
                    break;
                case IntegerLiteralAtom atom:
                    WaddleIntegerLiteral(atom);
                    break;
                case StringLiteralAtom atom:
                    WaddleStringLiteral(atom);
                    break;
                case IdentifierAtom atom:
                    WaddleIdentifier(atom);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(exprStmt));
            };
        }

        private void WaddleIdentifier(IdentifierAtom atom)
        {
            _listener.EnterIdentifierLiteral(atom);
            _listener.LeaveIdentifierLiteral(atom);
        }

        private void WaddleStringLiteral(StringLiteralAtom atom)
        {
            _listener.EnterStringLiteral(atom);
            _listener.LeaveStringLiteral(atom);
        }

        private void WaddleIntegerLiteral(IntegerLiteralAtom atom)
        {
            _listener.EnterIntegerLiteral(atom);
            _listener.LeaveIntegerLiteral(atom);
        }

        private void WaddleBoolLiteral(BoolLiteralAtom atom)
        {
            _listener.EnterBoolLiteral(atom);
            _listener.LeaveBoolLiteral(atom);
        }

        private void WaddleTermExpr(TermExpressionSyntax termExpr)
        {
            _listener.EnterTermExpr(termExpr);
            WaddleExpression(termExpr.Left);
            WaddleExpression(termExpr.Right);
            _listener.LeaveTermExpr(termExpr);
        }

        private void WaddleProductExpr(ProductExpressionSyntax productExpr)
        {
            _listener.EnterProductExpr(productExpr);
            WaddleExpression(productExpr.Left);
            WaddleExpression(productExpr.Right);
            _listener.LeaveProductExpr(productExpr);
        }

        private void WaddleLogicalExpr(LogicalExpressionSyntax logicalExpr)
        {
            _listener.EnterLogicalExpr(logicalExpr);
            WaddleExpression(logicalExpr.Left);
            WaddleExpression(logicalExpr.Right);
            _listener.LeaveLogicalExpr(logicalExpr);
        }

        private void WaddleRelationalExpr(RelationalExpressionSyntax relationalExpr)
        {
            _listener.EnterRelationalExpr(relationalExpr);
            WaddleExpression(relationalExpr.Left);
            WaddleExpression(relationalExpr.Right);
            _listener.LeaveRelationalExpr(relationalExpr);
        }

        private void WaddleInvocationExpr(InvocationExpressionSyntax invocationExpr)
        {
            _listener.EnterInvocationExpr(invocationExpr);
            foreach (var argument in invocationExpr.Arguments)
            {
                WaddleExpression(argument);
            }
            _listener.LeaveInvocationExpr(invocationExpr);
        }
    }
}
