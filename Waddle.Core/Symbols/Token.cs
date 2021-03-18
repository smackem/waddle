namespace Waddle.Core.Symbols {
    public struct Token {
        private readonly TokenType type;
        private readonly string lexeme; // if type == TokenType.Identifier -> printHello
        private readonly int lineNumber;
        private readonly int charPosition;

        public Token() { }
        
        public Token(TokenType type, string lexeme, int lineNumber, int charPosition)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.lineNumber = lineNumber;
            this.charPosition = charPosition;
        }
    }    
}
