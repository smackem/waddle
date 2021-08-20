namespace Waddle.Core.Symbols
{
    public record Token(TokenType Type, string Lexeme, int LineNumber, int CharPosition);
}
