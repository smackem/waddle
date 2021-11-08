using System;

namespace Waddle.Core.Syntax.Ast
{
    public class SemanticWaddleListener : IWaddleListener
    {
        public void EnterProgram(ProgramSyntax syntax)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveProgram(ProgramSyntax syntax)
        {
            throw new System.NotImplementedException();
        }

        public void EnterFunctionDeclaration(FunctionDeclSyntax syntax)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveFunctionDeclaration(FunctionDeclSyntax syntax)
        {
            throw new System.NotImplementedException();
        }

        public void EnterPrintStmt(PrintStmtSyntax syntax)
        {
            throw new System.NotImplementedException();
        }

        public void LeavePrintStmt(PrintStmtSyntax syntax)
        {
            throw new System.NotImplementedException();
        }

        public void EnterIdentifierLiteral(IdentifierAtom atom)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveIdentifierLiteral(IdentifierAtom atom)
        {
            throw new System.NotImplementedException();
        }

        public void EnterBoolLiteral(BoolLiteralAtom atom)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveBoolLiteral(BoolLiteralAtom atom)
        {
            throw new System.NotImplementedException();
        }

        public void EnterIntegerLiteral(IntegerLiteralAtom atom)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveIntegerLiteral(IntegerLiteralAtom atom)
        {
            throw new System.NotImplementedException();
        }

        public void EnterStringLiteral(StringLiteralAtom atom)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveStringLiteral(StringLiteralAtom atom)
        {
            throw new System.NotImplementedException();
        }

        public void EnterTermExpr(TermExpressionSyntax termExpr)
        {
            var visitor = new TypeExtractingVisitor(); // this should be a member

            if (termExpr.Left.Accept(visitor) != termExpr.Right.Accept(visitor))
            {
                throw new SemanticErrorException("incompatible types");
            }
        }

        public void LeaveTermExpr(TermExpressionSyntax termExpr)
        {
            throw new System.NotImplementedException();
        }

        public void EnterProductExpr(ProductExpressionSyntax productExpr)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveProductExpr(ProductExpressionSyntax productExpr)
        {
            throw new System.NotImplementedException();
        }

        public void EnterRelationalExpr(RelationalExpressionSyntax relationalExpr)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveRelationalExpr(RelationalExpressionSyntax relationalExpr)
        {
            throw new System.NotImplementedException();
        }

        public void EnterLogicalExpr(LogicalExpressionSyntax logicalExpr)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveLogicalExpr(LogicalExpressionSyntax logicalExpr)
        {
            throw new System.NotImplementedException();
        }

        public void EnterInvocationExpr(InvocationExpressionSyntax invocationExpr)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveInvocationExpr(InvocationExpressionSyntax invocationExpr)
        {
            throw new System.NotImplementedException();
        }

        public void EnterIfStmt(IfStmtSyntax ifStmt)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveIfStmt(IfStmtSyntax ifStmt)
        {
            throw new System.NotImplementedException();
        }

        public void EnterReturnStmt(ReturnStmtSyntax returnStmt)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveReturnStmt(ReturnStmtSyntax returnStmt)
        {
            throw new System.NotImplementedException();
        }

        public void EnterAssignStmt(AssignStmtSyntax assignStmt)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveAssignStmt(AssignStmtSyntax assignStmt)
        {
            throw new System.NotImplementedException();
        }

        public void EnterDeclStmt(DeclStmtSyntax declStmt)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveDeclStmt(DeclStmtSyntax declStmt)
        {
            throw new System.NotImplementedException();
        }

        public void EnterBlock(BlockSyntax block)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveBlock(BlockSyntax block)
        {
            throw new System.NotImplementedException();
        }
    }
}