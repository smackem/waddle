using System;

namespace Waddle.Core.Syntax.Ast
{
    public class SemanticWaddleListener : IWaddleListener
    {
        public bool EnterProgram(ProgramSyntax syntax, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveProgram(ProgramSyntax syntax, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterFunctionDeclaration(FunctionDeclSyntax syntax, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveFunctionDeclaration(FunctionDeclSyntax syntax, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterPrintStmt(PrintStmtSyntax syntax, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeavePrintStmt(PrintStmtSyntax syntax, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterIdentifierLiteral(IdentifierAtom atom, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveIdentifierLiteral(IdentifierAtom atom, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterBoolLiteral(BoolLiteralAtom atom, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveBoolLiteral(BoolLiteralAtom atom, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterIntegerLiteral(IntegerLiteralAtom atom, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveIntegerLiteral(IntegerLiteralAtom atom, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterStringLiteral(StringLiteralAtom atom, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveStringLiteral(StringLiteralAtom atom, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterTermExpr(TermExpressionSyntax termExpr, WaddleContext ctx)
        {
            var visitor = new TypeExtractingVisitor(); // this should be a member

            if (termExpr.Left.Accept(visitor) != termExpr.Right.Accept(visitor))
            {
                throw new SemanticErrorException("incompatible types");
            }
        }

        public void LeaveTermExpr(TermExpressionSyntax termExpr, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterProductExpr(ProductExpressionSyntax productExpr, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveProductExpr(ProductExpressionSyntax productExpr, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterRelationalExpr(RelationalExpressionSyntax relationalExpr, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveRelationalExpr(RelationalExpressionSyntax relationalExpr, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterLogicalExpr(LogicalExpressionSyntax logicalExpr, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveLogicalExpr(LogicalExpressionSyntax logicalExpr, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterInvocationExpr(InvocationExpressionSyntax invocationExpr, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveInvocationExpr(InvocationExpressionSyntax invocationExpr, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterIfStmt(IfStmtSyntax ifStmt, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveIfStmt(IfStmtSyntax ifStmt, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterReturnStmt(ReturnStmtSyntax returnStmt, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveReturnStmt(ReturnStmtSyntax returnStmt, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterAssignStmt(AssignStmtSyntax assignStmt, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveAssignStmt(AssignStmtSyntax assignStmt, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterDeclStmt(DeclStmtSyntax declStmt, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveDeclStmt(DeclStmtSyntax declStmt, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public bool EnterBlock(BlockSyntax block, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveBlock(BlockSyntax block, WaddleContext ctx)
        {
            throw new System.NotImplementedException();
        }
    }
}