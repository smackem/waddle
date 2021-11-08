using System;
using System.Collections.Generic;
using System.Linq;

namespace Waddle.Core.Syntax.Ast
{
    public class Waddler
    {
        private readonly IWaddleListener _listener;
        private readonly Stack<Syntax> _stack = new();
        private readonly WaddleContext _ctx;

        public Waddler(IWaddleListener listener)
        {
            _listener = listener;
            _ctx = new WaddleContext(_stack);
        }

        public void WaddleProgram(ProgramSyntax program)
        {
            _stack.Push(program);

            if (_listener.EnterProgram(program, _ctx))
            {
                foreach (var function in program.FunctionDeclarations)
                {
                    WaddleFunctionDeclaration(function);
                }

                _listener.LeaveProgram(program, _ctx);
            }

            _stack.Pop();
        }

        private void WaddleFunctionDeclaration(FunctionDeclSyntax function)
        {
            _stack.Push(function);

            if (_listener.EnterFunctionDeclaration(function, _ctx))
            {
                WaddleBlock(function.Body);

                _listener.LeaveFunctionDeclaration(function, _ctx);
            }

            _stack.Pop();
        }

        private void WaddleBlock(BlockSyntax block)
        {
            _stack.Push(block);

            if (_listener.EnterBlock(block, _ctx))
            {
                foreach (var stmt in block.Statements)
                {
                    WaddleStmt(stmt);
                }
            
                _listener.LeaveBlock(block, _ctx);
            }

            _stack.Pop();
        }

        private void WaddleStmt(StatementSyntax stmt)
        {
            _stack.Push(stmt);

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

            _stack.Pop();
        }

        private void WaddlePrintStmt(PrintStmtSyntax printStmt)
        {
            if (_listener.EnterPrintStmt(printStmt, _ctx) == false)
            {
                return;
            }

            foreach (var printStmtArgument in printStmt.Arguments)
            {
                WaddleExpression(printStmtArgument);
            }

            _listener.LeavePrintStmt(printStmt, _ctx);
        }

        private void WaddleIfStmt(IfStmtSyntax ifStmt)
        {
            if (_listener.EnterIfStmt(ifStmt, _ctx) == false)
            {
                return;
            }

            WaddleExpression(ifStmt.Expression);
            WaddleBlock(ifStmt.Body);

            _listener.LeaveIfStmt(ifStmt, _ctx);
        }

        private void WaddleReturnStmt(ReturnStmtSyntax returnStmt)
        {
            if (_listener.EnterReturnStmt(returnStmt, _ctx) == false)
            {
                return;
            }

            WaddleExpression(returnStmt.Expression);

            _listener.LeaveReturnStmt(returnStmt, _ctx);
        }

        private void WaddleAssignStmt(AssignStmtSyntax assignStmt)
        {
            if (_listener.EnterAssignStmt(assignStmt, _ctx) == false)
            {
                return;
            }

            WaddleExpression(assignStmt.Expression);

            _listener.LeaveAssignStmt(assignStmt, _ctx);
        }

        private void WaddleDeclStmt(DeclStmtSyntax declStmt)
        {
            if (_listener.EnterDeclStmt(declStmt, _ctx) == false)
            {
                return;
            }

            WaddleExpression(declStmt.Expression);

            _listener.LeaveDeclStmt(declStmt, _ctx);
        }

        private void WaddleExpression(ExpressionSyntax expr)
        {
            _stack.Push(expr);

            switch (expr)
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
                    throw new ArgumentOutOfRangeException(nameof(expr));
            };

            _stack.Pop();
        }

        private void WaddleIdentifier(IdentifierAtom atom)
        {
            if (_listener.EnterIdentifierLiteral(atom, _ctx) == false)
            {
                return;
            }

            _listener.LeaveIdentifierLiteral(atom, _ctx);
        }

        private void WaddleStringLiteral(StringLiteralAtom atom)
        {
            if (_listener.EnterStringLiteral(atom, _ctx) == false)
            {
                return;
            }

            _listener.LeaveStringLiteral(atom, _ctx);
        }

        private void WaddleIntegerLiteral(IntegerLiteralAtom atom)
        {
            if (_listener.EnterIntegerLiteral(atom, _ctx) == false)
            {
                return;
            }

            _listener.LeaveIntegerLiteral(atom, _ctx);
        }

        private void WaddleBoolLiteral(BoolLiteralAtom atom)
        {
            if (_listener.EnterBoolLiteral(atom, _ctx) == false)
            {
                return;
            }

            _listener.LeaveBoolLiteral(atom, _ctx);
        }

        private void WaddleTermExpr(TermExpressionSyntax termExpr)
        {
            if (_listener.EnterTermExpr(termExpr, _ctx) == false)
            {
                return;
            }

            WaddleExpression(termExpr.Left);
            WaddleExpression(termExpr.Right);
            _listener.LeaveTermExpr(termExpr, _ctx);
        }

        private void WaddleProductExpr(ProductExpressionSyntax productExpr)
        {
            if (_listener.EnterProductExpr(productExpr, _ctx) == false)
            {
                return;
            }

            WaddleExpression(productExpr.Left);
            WaddleExpression(productExpr.Right);
            _listener.LeaveProductExpr(productExpr, _ctx);
        }

        private void WaddleLogicalExpr(LogicalExpressionSyntax logicalExpr)
        {
            if (_listener.EnterLogicalExpr(logicalExpr, _ctx) == false)
            {
                return;
            }

            WaddleExpression(logicalExpr.Left);
            WaddleExpression(logicalExpr.Right);
            _listener.LeaveLogicalExpr(logicalExpr, _ctx);
        }

        private void WaddleRelationalExpr(RelationalExpressionSyntax relationalExpr)
        {
            if (_listener.EnterRelationalExpr(relationalExpr, _ctx) == false)
            {
                return;
            }

            WaddleExpression(relationalExpr.Left);
            WaddleExpression(relationalExpr.Right);
            _listener.LeaveRelationalExpr(relationalExpr, _ctx);
        }

        private void WaddleInvocationExpr(InvocationExpressionSyntax invocationExpr)
        {
            if (_listener.EnterInvocationExpr(invocationExpr, _ctx) == false)
            {
                return;
            }

            foreach (var argument in invocationExpr.Arguments)
            {
                WaddleExpression(argument);
            }

            _listener.LeaveInvocationExpr(invocationExpr, _ctx);
        }
    }
}
