namespace Waddle.Core.Syntax.Ast
{
    public interface ISyntaxVisitor<in TState, out TResult>
    {
        TResult Visit(ProgramSyntax syntax, TState state);
        TResult Visit(FunctionDeclSyntax syntax, TState state);
        TResult Visit(TypeSyntax syntax, TState state);
        TResult Visit(ParameterDeclSyntax syntax, TState state);
        TResult Visit(BlockSyntax syntax, TState state);
        TResult Visit(ReturnStmtSyntax syntax, TState state);
        TResult Visit(IfStmtSyntax syntax, TState state);
        TResult Visit(AssignStmtSyntax syntax, TState state);
        TResult Visit(PrintStmtSyntax syntax, TState state);
        TResult Visit(DeclStmtSyntax syntax, TState state);
        TResult Visit(InvocationStmtSyntax syntax, TState state);
        TResult Visit(TermExpressionSyntax syntax, TState state);
        TResult Visit(LogicalExpressionSyntax syntax, TState state);
        TResult Visit(RelationalExpressionSyntax syntax, TState state);
        TResult Visit(ProductExpressionSyntax syntax, TState state);
        TResult Visit(InvocationExpressionSyntax syntax, TState state);
        TResult Visit(IntegerLiteralAtom syntax, TState state);
        TResult Visit(BoolLiteralAtom syntax, TState state);
        TResult Visit(StringLiteralAtom syntax, TState state);
        TResult Visit(IdentifierAtom syntax, TState state);
    }
}