using System.Collections.Generic;
using Waddle.Core.Symbols;

namespace Waddle.Core.Syntax.Ast
{
    public abstract record Syntax(Token StartToken);

    public record ProgramSyntax(Token StartToken, IEnumerable<FunctionDeclSyntax> FunctionDecls) : Syntax(StartToken);

    public record FunctionDeclSyntax(Token StartToken, string Name, ParameterListSyntax? Parameters,
        TypeSyntax ReturnType, BlockSyntax Body);

    public record BlockSyntax(Token StartToken, IEnumerable<StatementSyntax> Stmts) : Syntax(StartToken);

    public abstract record StatementSyntax(Token StartToken) : Syntax(StartToken);

    public record ReturnStmtSyntax(Token StartToken, ExpressionSyntax Expr) : StatementSyntax(StartToken);
}