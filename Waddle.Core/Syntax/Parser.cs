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
            Assert(TokenType.Function);
            Expect(TokenType.Identifier);
            Expect(TokenType.LParen);
            Next();
            while (CurrentToken.Type != TokenType.RParen)
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
            switch (token.Type)
            {
                case TokenType.Return:
                    ParseReturnStatement();
                    break;
                case TokenType.If:
                    ParseIfStatement();
                    break;
                case TokenType.Identifier:
                    ParseAssignStatement();
                    break;
                case TokenType.Print:
                    ParsePrintStatement();
                    break;
                case TokenType.Var:
                    ParseDeclStatement();
                    break;
                case TokenType.Arrow:
                    ParseInvocationStatement();
                    break;
                default:
                    throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - Statement expected ");
            }
        }

        private void ParseDeclStatement()
        {
            ParseParameter();
            Expect(TokenType.Equal);
            ParseExpression();
        }

        private void ParsePrintStatement()
        {
            Expect(TokenType.LParen);
            while (Next().Type != TokenType.RParen)
            {
                ParseExpression();
            }
        }

        private void ParseAssignStatement()
        {
            Expect(TokenType.Equal);
            ParseExpression();
        }

        private void ParseInvocationStatement()
        {
            ParseInvocationExpression();
        }

        private void ParseReturnStatement()
        {
            ParseExpression();
        }

        private void ParseIfStatement()
        {
            ParseExpression();
            ParseBlock();
        }

        private void ParseExpression()
        {
            ParseLogicalOrExpression();
        }

        private void ParseLogicalOrExpression()
        {
            ParseLogicalAndExpression();
            while (true)
            {
                if (CurrentToken.Type != TokenType.Or)
                {
                    return;
                }

                Next();
                ParseLogicalAndExpression();
            }
        }

        private void ParseLogicalAndExpression()
        {
            ParseRelationalExpression();
            while (true)
            {
                if (CurrentToken.Type != TokenType.And)
                {
                    return;
                }

                Next();
                ParseRelationalExpression();
            }
        }

        private void ParseRelationalExpression()
        {
            ParseTermExpression();
            if (IsRelationalOperator(CurrentToken) == false)
            {
                return;
            }

            Next();
            ParseTermExpression();
        }

        private static bool IsRelationalOperator(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Equals:
                case TokenType.NotEquals:
                case TokenType.LessEquals:
                case TokenType.LessThan:
                case TokenType.GreaterEquals:
                case TokenType.GreaterThan:
                    return true;
            }

            return false;
        }

        private void ParseTermExpression()
        {
            ParseProductExpression();
            while (true)
            {
                if (IsTermOperator(CurrentToken) == false)
                {
                    return;
                }

                Next();
                ParseProductExpression();
            }
        }

        private static bool IsTermOperator(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                    return true;
            }

            return false;
        }

        private void ParseProductExpression()
        {
            ParseAtom();
            while (true)
            {
                if (IsProductOperator(CurrentToken) == false)
                {
                    return;
                }

                Next();
                ParseAtom();
            }
        }

        private static bool IsProductOperator(Token currentToken)
        {
            return currentToken.Type is TokenType.Multiply or TokenType.Divide;
        }

        private void ParseAtom()
        {
            var token = Next();
            if (token.Type == TokenType.Arrow)
            {
                ParseInvocationExpression();
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

        private void ParseInvocationExpression()
        {
            Expect(TokenType.Identifier);
            Expect(TokenType.LParen);
            while (Next().Type != TokenType.RParen)
            {
                ParseExpression();
            }
        }

        private void ParseParameter()
        {
            Assert(TokenType.Identifier);
            Expect(TokenType.Colon);
            ExpectTypeToken();
            Next();
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

        private void Assert(TokenType tokenType)
        {
            var token = CurrentToken;
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