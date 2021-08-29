using System;
using System.Collections.Generic;
using System.Linq;
using Waddle.Core.Symbols;

namespace Waddle.Core.Syntax
{
    public class Parser
    {
        private readonly Token[] _tokens;
        private IEnumerator<Token> _enumerator;
        private int _index = -1;

        public Parser(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToArray();
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
            ExpectCurrent(TokenType.Function);
            Expect(TokenType.Identifier);
            Expect(TokenType.LParen);
            while (Next().Type != TokenType.RParen)
            {
                ParseParameter();
            }

            if (Next().Type == TokenType.Arrow)
            {
                ExpectTypeToken();
            }
            
            ParseBlock();
            // CurrentToken.Lexeme
        }

        private void ExpectCurrent(TokenType tokenType)
        {
            if (CurrentToken.Type != tokenType)
            {
                throw new Exception($"syntax error @({CurrentToken.LineNumber}:{CurrentToken.CharPosition} - expected {tokenType}");
            }
        }

        private void ParseBlock()
        {
            if (CurrentToken.Type != TokenType.LBrace)
            {
                Expect(TokenType.LBrace);
            }
            
            while (Next().Type != TokenType.RBrace)
            {
                ParseStatement();
            }
        }

        private void ParseStatement()
        {   
            var token = CurrentToken;
            if (token.Type == TokenType.Return)
            {
                ParseExpression();
            }
            else if (token.Type == TokenType.If)
            {
                ParseExpression();
                ParseBlock();
            }
            else if (token.Type == TokenType.Identifier)
            {
                Expect(TokenType.Equal);
                ParseExpression();
            }
            else if (token.Type == TokenType.Print)
            {
                Expect(TokenType.LParen);
                while (Next().Type != TokenType.RParen)
                {
                    ParseExpression();
                }
            }
            else if (token.Type == TokenType.Var)
            {
                ParseParameter();
                Expect(TokenType.Equal);
                ParseExpression();
            }
            else if (token.Type == TokenType.Arrow)
            {
                Expect(TokenType.Identifier);
                Expect(TokenType.LParen);
                while (Next().Type != TokenType.RParen)
                {
                    ParseExpression();
                }
            }
            else
            {
                throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - Statement expected ");   
            }
        }

        private void ParseExpression()
        {
            ParseAtom();
            var token = _tokens[_index + 1];
            if (IsOperator(token))
            {
                ParseExpression();
            }
        }

        private bool IsOperator(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Equals:
                case TokenType.NotEquals:
                case TokenType.LessEquals:
                case TokenType.LessThan:
                case TokenType.GreaterEquals:
                case TokenType.GreaterThan:
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Multiply:
                case TokenType.Divide:
                case TokenType.And:
                case TokenType.Or:
                    return true;
            }

            return false;
        }

        private void ParseAtom()
        {
            var token = Next();
            if (token.Type == TokenType.Arrow)
            {
                Expect(TokenType.Identifier);
                Expect(TokenType.LParen);
                while (Next().Type != TokenType.RParen)
                {
                    ParseExpression();
                }
            }
            else if (token.Type == TokenType.Number)
            {
                //nichts weiter zu lesen   
            }
            else if (token.Type == TokenType.Identifier)
            {
                //nichts weiter zu lesen   
            }
            else
            {
                throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - atom expected ");   
            }
        }

        private void ParseParameter()
        {
            Expect(TokenType.Identifier);
            Expect(TokenType.Colon);
            ExpectTypeToken();
        }

        private void ExpectTypeToken()
        {
            var token = Next();
            switch (token.Type)
            {
                case TokenType.Bool:
                case TokenType.String:
                case TokenType.Float:
                case TokenType.Int:
                case TokenType.Char:
                    return;
            }
            throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - Type expected ");
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
            _index++;
            return _enumerator.MoveNext()
                ? _enumerator.Current
                : new Token(TokenType.Eof, string.Empty, 0, 0);
        }
    }
}