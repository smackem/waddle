using System;
using System.Collections.Generic;

namespace Waddle.Core.Syntax.Ast
{
    public interface IWaddleListener
    {
        bool EnterProgram(ProgramSyntax syntax, WaddleContext ctx);
        void LeaveProgram(ProgramSyntax syntax, WaddleContext ctx);
        bool EnterFunctionDeclaration(FunctionDeclSyntax syntax, WaddleContext ctx);
        void LeaveFunctionDeclaration(FunctionDeclSyntax syntax, WaddleContext ctx);
        bool EnterPrintStmt(PrintStmtSyntax syntax, WaddleContext ctx);
        void LeavePrintStmt(PrintStmtSyntax syntax, WaddleContext ctx);
        bool EnterIdentifierLiteral(IdentifierAtom atom, WaddleContext ctx);
        void LeaveIdentifierLiteral(IdentifierAtom atom, WaddleContext ctx);
        bool EnterBoolLiteral(BoolLiteralAtom atom, WaddleContext ctx);
        void LeaveBoolLiteral(BoolLiteralAtom atom, WaddleContext ctx);
        bool EnterIntegerLiteral(IntegerLiteralAtom atom, WaddleContext ctx);
        void LeaveIntegerLiteral(IntegerLiteralAtom atom, WaddleContext ctx);
        bool EnterStringLiteral(StringLiteralAtom atom, WaddleContext ctx);
        void LeaveStringLiteral(StringLiteralAtom atom, WaddleContext ctx);
        bool EnterTermExpr(TermExpressionSyntax termExpr, WaddleContext ctx);
        void LeaveTermExpr(TermExpressionSyntax termExpr, WaddleContext ctx);
        bool EnterProductExpr(ProductExpressionSyntax productExpr, WaddleContext ctx);
        void LeaveProductExpr(ProductExpressionSyntax productExpr, WaddleContext ctx);
        bool EnterRelationalExpr(RelationalExpressionSyntax relationalExpr, WaddleContext ctx);
        void LeaveRelationalExpr(RelationalExpressionSyntax relationalExpr, WaddleContext ctx);
        bool EnterLogicalExpr(LogicalExpressionSyntax logicalExpr, WaddleContext ctx);
        void LeaveLogicalExpr(LogicalExpressionSyntax logicalExpr, WaddleContext ctx);
        bool EnterInvocationExpr(InvocationExpressionSyntax invocationExpr, WaddleContext ctx);
        void LeaveInvocationExpr(InvocationExpressionSyntax invocationExpr, WaddleContext ctx);
        bool EnterIfStmt(IfStmtSyntax ifStmt, WaddleContext ctx);
        void LeaveIfStmt(IfStmtSyntax ifStmt, WaddleContext ctx);
        bool EnterReturnStmt(ReturnStmtSyntax returnStmt, WaddleContext ctx);
        void LeaveReturnStmt(ReturnStmtSyntax returnStmt, WaddleContext ctx);
        bool EnterAssignStmt(AssignStmtSyntax assignStmt, WaddleContext ctx);
        void LeaveAssignStmt(AssignStmtSyntax assignStmt, WaddleContext ctx);
        bool EnterDeclStmt(DeclStmtSyntax declStmt, WaddleContext ctx);
        void LeaveDeclStmt(DeclStmtSyntax declStmt, WaddleContext ctx);
        bool EnterBlock(BlockSyntax block, WaddleContext ctx);
        void LeaveBlock(BlockSyntax block, WaddleContext ctx);
    }
}