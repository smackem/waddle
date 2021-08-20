using System;
using System.Collections.Generic;
using Waddle.Core.Symbols;

namespace Waddle.Core.Syntax
{
    public class Parser
    {
        private readonly IEnumerator<Token> _enumerator;

        public Parser(IEnumerable<Token> tokens)
        {
            _enumerator = tokens.GetEnumerator();
        }

        public void Parse()
        {
            ParseProgram();
        }

        private void ParseProgram()
        {
            while (Next().Type != TokenType.Eof)
            {
                ParseFunctionDecl();
            }
        }

        private void ParseFunctionDecl()
        {
            Expect(TokenType.Function);
            Expect(TokenType.Identifier);
            // CurrentToken.Lexeme
        }

        private void Expect(TokenType tokenType)
        {
            var token = Next();
            if (token.Type != tokenType)
            {
                throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - expected {tokenType}");
            }
        }

        private Token CurrentToken => _enumerator.Current;

        private Token Next()
        {
            return _enumerator.MoveNext()
                ? _enumerator.Current
                : new Token(TokenType.Eof, string.Empty, 0, 0);
        }
    }
}