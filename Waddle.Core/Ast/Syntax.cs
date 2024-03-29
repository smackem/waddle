using System.Collections.Generic;
using Waddle.Core.Lexing;

namespace Waddle.Core.Ast
{
    public abstract record Syntax(Token StartToken)
    {
        public abstract TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor);
    }

    public record ProgramSyntax
        (Token StartToken, IEnumerable<FunctionDeclSyntax> FunctionDeclarations) : Syntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record FunctionDeclSyntax(Token StartToken, string Name, IEnumerable<ParameterDeclSyntax> Parameters, TypeSyntax? ReturnType, BlockSyntax Body)
        : Syntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record TypeSyntax(Token StartToken, Token TypeToken) : Syntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record ParameterDeclSyntax(Token StartToken, string Name, Token ColonToken, TypeSyntax TypeSyntax) : Syntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record BlockSyntax(Token StartToken, IEnumerable<StatementSyntax> Statements) : Syntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public abstract record StatementSyntax(Token StartToken) : Syntax(StartToken);

    public record ReturnStmtSyntax(Token StartToken, ExpressionSyntax Expression) : StatementSyntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record IfStmtSyntax (Token StartToken, ExpressionSyntax Expression, BlockSyntax Body) : StatementSyntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record AssignStmtSyntax (Token StartToken, Token EqualToken, ExpressionSyntax Expression) : StatementSyntax(StartToken)
    {
        public string Identifier => StartToken.Lexeme;

        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record PrintStmtSyntax (Token StartToken, Token LParenToken, IEnumerable<ExpressionSyntax> Arguments) : StatementSyntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record DeclStmtSyntax(Token StartToken, ParameterDeclSyntax ParameterDeclSyntax, Token EqualToken,  ExpressionSyntax Expression) : StatementSyntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record InvocationStmtSyntax(Token StartToken, InvocationExpressionSyntax Expression) : StatementSyntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public abstract record ExpressionSyntax(Token StartToken) : Syntax(StartToken);

    public abstract record BinaryExpressionSyntax(Token StartToken, ExpressionSyntax Left, ExpressionSyntax Right): ExpressionSyntax(StartToken);

    public record TermExpressionSyntax(Token StartToken, ExpressionSyntax Left, ExpressionSyntax Right, TermOperator Operator)
        : BinaryExpressionSyntax(StartToken, Left, Right)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public enum TermOperator
    {
        Plus,
        Minus
    }

    public record LogicalExpressionSyntax(Token StartToken, ExpressionSyntax Left, ExpressionSyntax Right, LogicalOperator Operator)
        : BinaryExpressionSyntax(StartToken, Left, Right)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public enum LogicalOperator
    {
        And,
        Or,
    }

    public record RelationalExpressionSyntax(Token StartToken, ExpressionSyntax Left, ExpressionSyntax Right, RelationalOperator Operator)
        : BinaryExpressionSyntax(StartToken, Left, Right)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

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
        : BinaryExpressionSyntax(StartToken, Left, Right)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

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
        Token RParen) : AtomSyntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record IntegerLiteralAtom(Token StartToken, int Value) : AtomSyntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record BoolLiteralAtom(Token StartToken, bool Value) : AtomSyntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record StringLiteralAtom(Token StartToken, string Value) : AtomSyntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record IdentifierAtom(Token StartToken, string Identifier) : AtomSyntax(StartToken)
    {
        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
