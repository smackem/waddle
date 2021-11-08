using System;
using System.Collections.Generic;

namespace Waddle.Core.Syntax.Ast
{
    // TODO: add WaddleContext parameter with list of node ancestors
    public interface IWaddleListener
    {
        void EnterProgram(ProgramSyntax syntax);
        void LeaveProgram(ProgramSyntax syntax);
        void EnterFunctionDeclaration(FunctionDeclSyntax syntax);
        void LeaveFunctionDeclaration(FunctionDeclSyntax syntax);
        void EnterPrintStmt(PrintStmtSyntax syntax);
        void LeavePrintStmt(PrintStmtSyntax syntax);
        void EnterIdentifierLiteral(IdentifierAtom atom);
        void LeaveIdentifierLiteral(IdentifierAtom atom);
        void EnterBoolLiteral(BoolLiteralAtom atom);
        void LeaveBoolLiteral(BoolLiteralAtom atom);
        void EnterIntegerLiteral(IntegerLiteralAtom atom);
        void LeaveIntegerLiteral(IntegerLiteralAtom atom);
        void EnterStringLiteral(StringLiteralAtom atom);
        void LeaveStringLiteral(StringLiteralAtom atom);
        void EnterTermExpr(TermExpressionSyntax termExpr);
        void LeaveTermExpr(TermExpressionSyntax termExpr);
        void EnterProductExpr(ProductExpressionSyntax productExpr);
        void LeaveProductExpr(ProductExpressionSyntax productExpr);
        void EnterRelationalExpr(RelationalExpressionSyntax relationalExpr);
        void LeaveRelationalExpr(RelationalExpressionSyntax relationalExpr);
        void EnterLogicalExpr(LogicalExpressionSyntax logicalExpr);
        void LeaveLogicalExpr(LogicalExpressionSyntax logicalExpr);
        void EnterInvocationExpr(InvocationExpressionSyntax invocationExpr);
        void LeaveInvocationExpr(InvocationExpressionSyntax invocationExpr);
        void EnterIfStmt(IfStmtSyntax ifStmt);
        void LeaveIfStmt(IfStmtSyntax ifStmt);
        void EnterReturnStmt(ReturnStmtSyntax returnStmt);
        void LeaveReturnStmt(ReturnStmtSyntax returnStmt);
        void EnterAssignStmt(AssignStmtSyntax assignStmt);
        void LeaveAssignStmt(AssignStmtSyntax assignStmt);
        void EnterDeclStmt(DeclStmtSyntax declStmt);
        void LeaveDeclStmt(DeclStmtSyntax declStmt);
        void EnterBlock(BlockSyntax block);
        void LeaveBlock(BlockSyntax block);
    }
}