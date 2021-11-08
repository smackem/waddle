namespace Waddle.Core.Syntax.Ast
{
    public interface ISyntaxVisitor<out TResult>
    {
        TResult Visit(ProgramSyntax syntax);
        TResult Visit(FunctionDeclSyntax syntax);
        TResult Visit(TypeSyntax syntax);
        TResult Visit(ParameterDeclSyntax syntax);
        TResult Visit(BlockSyntax syntax);
        TResult Visit(ReturnStmtSyntax syntax);
        TResult Visit(IfStmtSyntax syntax);
        TResult Visit(AssignStmtSyntax syntax);
        TResult Visit(PrintStmtSyntax syntax);
        TResult Visit(DeclStmtSyntax syntax);
        TResult Visit(InvocationStmtSyntax syntax);
        TResult Visit(TermExpressionSyntax syntax);
        TResult Visit(LogicalExpressionSyntax syntax);
        TResult Visit(RelationalExpressionSyntax syntax);
        TResult Visit(ProductExpressionSyntax syntax);
        TResult Visit(InvocationExpressionSyntax syntax);
        TResult Visit(IntegerLiteralAtom syntax);
        TResult Visit(BoolLiteralAtom syntax);
        TResult Visit(StringLiteralAtom syntax);
        TResult Visit(IdentifierAtom syntax);
    }
}