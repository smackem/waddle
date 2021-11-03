namespace Waddle.Core.Lexing
{
    public record Token(TokenType Type, string Lexeme, int LineNumber, int CharPosition);
}
