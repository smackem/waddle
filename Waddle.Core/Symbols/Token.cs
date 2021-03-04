namespace Waddle.Core.Symbols {
    public struct Token {
        private readonly TokenType type;
        private readonly string lexeme; // if type == TokenType.Identifier -> printHello
        private readonly int lineNumber;
        private readonly int charPosition;
    }
}
