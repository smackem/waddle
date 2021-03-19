namespace Waddle.Core.Symbols {
    public readonly struct Token {
        public TokenType Type { get; }

        public string Lexeme { get; }

        public int LineNumber { get; }

        public int CharPosition { get; }

        public Token(TokenType type, string lexeme, int lineNumber, int charPosition)
        {
            Type = type;
            Lexeme = lexeme;
            LineNumber = lineNumber;
            CharPosition = charPosition;
        }
    }    
}
