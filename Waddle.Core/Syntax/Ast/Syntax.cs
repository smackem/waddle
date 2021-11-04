using System.Collections.Generic;
using Waddle.Core.Lexing;

namespace Waddle.Core.Syntax.Ast
{
    public abstract record Syntax(Token StartToken)
    {
        public Syntax? Parent { get; internal set; }
    };

    public record ProgramSyntax
        (Token StartToken, IEnumerable<FunctionDeclSyntax> FunctionDeclarations) : Syntax(StartToken);

    public record FunctionDeclSyntax(Token StartToken, string Name, IEnumerable<ParameterDeclSyntax> Parameters, TypeSyntax? ReturnType, BlockSyntax Body)
        : Syntax(StartToken);

    public record TypeSyntax(Token StartToken, Token TypeToken) : Syntax(StartToken);
    
    public record ParameterDeclSyntax(Token StartToken, string Name, Token ColonToken, TypeSyntax TypeSyntax) : Syntax(StartToken);

    public record BlockSyntax(Token StartToken, IEnumerable<StatementSyntax> Statements) : Syntax(StartToken);

    public abstract record StatementSyntax(Token StartToken) : Syntax(StartToken);

    public record ReturnStmtSyntax(Token StartToken, ExpressionSyntax Expression) : StatementSyntax(StartToken);

    public record IfStmtSyntax (Token StartToken, ExpressionSyntax Expression, BlockSyntax Body) : StatementSyntax(StartToken);

    public record AssignStmtSyntax (Token StartToken, Token EqualToken, ExpressionSyntax Expression) : StatementSyntax(StartToken);

    public record PrintStmtSyntax (Token StartToken, Token LParenToken, IEnumerable<ExpressionSyntax> Arguments) : StatementSyntax(StartToken);

    public record DeclStmtSyntax(Token StartToken, ParameterDeclSyntax ParameterDeclSyntax, Token EqualToken,  ExpressionSyntax Expression) : StatementSyntax(StartToken);

    public record InvocationStmtSyntax(Token StartToken, InvocationExpressionSyntax Expression) : StatementSyntax(StartToken);

    public abstract record ExpressionSyntax(Token StartToken) : Syntax(StartToken);

    public abstract record BinaryExpressionSyntax(Token StartToken, ExpressionSyntax Left, ExpressionSyntax Right): ExpressionSyntax(StartToken);

    public record TermExpressionSyntax(Token StartToken, ExpressionSyntax Left, ExpressionSyntax Right, TermOperator Operator)
        : BinaryExpressionSyntax(StartToken, Left, Right);

    public enum TermOperator
    {
        Plus,
        Minus
    }

    public record LogicalExpressionSyntax(Token StartToken, ExpressionSyntax Left, ExpressionSyntax Right, LogicalOperator Operator)
        : BinaryExpressionSyntax(StartToken, Left, Right);

    public enum LogicalOperator
    {
        And,
        Or,
    }

    public record RelationalExpressionSyntax(Token StartToken, ExpressionSyntax Left, ExpressionSyntax Right, RelationalOperator Operator)
        : BinaryExpressionSyntax(StartToken, Left, Right);

    public enum RelationalOperator
    {
        Eq,
        Ne,
        Gt,
        Ge,
        Lt,
        Le,
    }

    public record ProductExpressionSyntax(Token StartToken, ExpressionSyntax Left, ExpressionSyntax Right, ProductOperator Operator)
        : BinaryExpressionSyntax(StartToken, Left, Right);

    public enum ProductOperator
    {
        Times,
        Divide,
    }

    public abstract record AtomSyntax(Token StartToken) : ExpressionSyntax(StartToken);

    public record InvocationExpressionSyntax(
        Token StartToken,
        string Identifier,
        Token LParen,
        IEnumerable<ExpressionSyntax> Arguments,
        Token RParen
    ) : AtomSyntax(StartToken);

    public record IntegerLiteralAtom(Token StartToken, int Value) : AtomSyntax(StartToken);
    
    public record BoolLiteralAtom(Token StartToken, bool Value) : AtomSyntax(StartToken);
    
    public record StringLiteralAtom(Token StartToken, string Value) : AtomSyntax(StartToken);

    public record IdentifierAtom(Token StartToken, string Identifier) : AtomSyntax(StartToken);
}