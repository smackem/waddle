namespace Waddle.Core.Ast
{
    public interface ISyntaxVisitor<out TResult>
    {
        TResult VisitProgram(ProgramSyntax syntax);
        TResult VisitFunctionDecl(FunctionDeclSyntax syntax);
        TResult VisitType(TypeSyntax syntax);
        TResult VisitParameterDecl(ParameterDeclSyntax syntax);
        TResult VisitBlock(BlockSyntax syntax);
        TResult VisitReturnStmt(ReturnStmtSyntax syntax);
        TResult VisitIfStmt(IfStmtSyntax syntax);
        TResult VisitAssignStmt(AssignStmtSyntax syntax);
        TResult VisitPrintStmt(PrintStmtSyntax syntax);
        TResult VisitDeclStmt(DeclStmtSyntax syntax);
        TResult VisitInvocationStmt(InvocationStmtSyntax syntax);
        TResult VisitTermExpr(TermExpressionSyntax syntax);
        TResult VisitLogicalExpr(LogicalExpressionSyntax syntax);
        TResult VisitRelationalExpr(RelationalExpressionSyntax syntax);
        TResult VisitProductExpr(ProductExpressionSyntax syntax);
        TResult VisitInvocationExpr(InvocationExpressionSyntax syntax);
        TResult VisitIntegerLiteral(IntegerLiteralAtom syntax);
        TResult VisitBoolLiteral(BoolLiteralAtom syntax);
        TResult VisitStringLiteral(StringLiteralAtom syntax);
        TResult VisitIdentifier(IdentifierAtom syntax);
    }
}