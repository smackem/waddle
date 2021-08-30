namespace Waddle.Core.Symbols
{
    public enum TokenType
    {
        Identifier,
        Number,
        StringLiteral,
        Function,
        LParen,
        RParen,
        LBrace,
        RBrace,
        Plus,
        Arrow,
        Minus,
        If,
        Colon,
        Semicolon,
        Comma,
        Print,
        Push,
        Length,
        Return,
        Equals,
        FString,
        For,
        Var,
        Match,
        And,
        Or,
        LessThan,
        GreaterThan,
        LessEquals,
        GreaterEquals,
        Int,
        Float,
        String,
        Bool,
        Char,
        Buffer,
        Regex,

        // ...
        Multiply,
        Divide,
        Equal,
        Dot,
        Eof,
        Unknown,
        NotEquals
    }
}