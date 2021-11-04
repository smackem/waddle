namespace Waddle.Core.Syntax.Ast
{
    public interface IWaddleListener<T>
    {
        T OnProgram(ProgramSyntax syntax);
        T OnFunctionDeclaration(FunctionDeclSyntax syntax);
        T OnPrintStmt(PrintStmtSyntax syntax);
    }
}