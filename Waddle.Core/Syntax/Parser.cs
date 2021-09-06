using System;
using System.Collections.Generic;
using Waddle.Core.Symbols;
using Waddle.Core.Syntax.Ast;

namespace Waddle.Core.Syntax
{
    public class Parser
    {
        private readonly IEnumerator<Token> _enumerator;

        public Parser(IEnumerable<Token> tokens)
        {
            _enumerator = tokens.GetEnumerator();
        }

        public Ast.Syntax Parse()
        {
            Next();
            return ParseProgram();
        }

        private Ast.Syntax ParseProgram()
        {
            var functionDecls = new List<FunctionDeclSyntax>();
            var startToken = CurrentToken;
            while (CurrentToken.Type != TokenType.Eof)
            {
                functionDecls.Add(ParseFunctionDecl());
            }

            return new ProgramSyntax(startToken, functionDecls);
        }

        private FunctionDeclSyntax ParseFunctionDecl()
        {
            Assert(TokenType.Function);
            Expect(TokenType.Identifier);
            Expect(TokenType.LParen);
            Next();
            while (CurrentToken.Type != TokenType.RParen)
            {
                ParseParameter();
            }

            Next();
            if (CurrentToken.Type == TokenType.Arrow)
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

            Next();
            while (CurrentToken.Type != TokenType.RBrace)
            {
                ParseStatement();
            }
            Next();
        }

        private void ParseStatement()
        {
            var token = CurrentToken;
            switch (token.Type)
            {
                case TokenType.Return:
                    ParseReturnStatement();
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
                case TokenType.If:
                    ParseIfStatement();
                    return;
                default:
                    throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - Statement expected - found {token.Type}");
            }
            Assert(TokenType.Semicolon);
            Next();
        }

        private void ParseDeclStatement()
        {
            Next();
            ParseParameter();
            Assert(TokenType.Equal);
            ParseExpression();
        }

        private void ParsePrintStatement()
        {
            Expect(TokenType.LParen);
            while (CurrentToken.Type != TokenType.RParen)
            {
                ParseExpression();
            }

            Next();
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
            Next();
            ParseAtom();
            while (true)
            {
                if (IsProductOperator(CurrentToken) == false)
                {
                    return;
                }

                ParseAtom();
            }
        }

        private static bool IsProductOperator(Token currentToken)
        {
            return currentToken.Type is TokenType.Multiply or TokenType.Divide;
        }

        private void ParseAtom()
        {
            if (CurrentToken.Type == TokenType.Arrow)
            {
                ParseInvocationExpression();
            }
            else if (CurrentToken.Type == TokenType.Number)
            {
                Next();
            }
            else if (CurrentToken.Type == TokenType.Identifier)
            {
                Next();
            }
            else
            {
                throw new Exception($"syntax error @({CurrentToken.LineNumber}:{CurrentToken.CharPosition} - atom expected - found {CurrentToken.Type}");   
            }
        }

        private void ParseInvocationExpression()
        {
            Expect(TokenType.Identifier);
            Expect(TokenType.LParen);
            while (CurrentToken.Type != TokenType.RParen)
            {
                ParseExpression();
            }
            Next();
        }

        private void ParseParameter()
        {
            Assert(TokenType.Identifier);
            Expect(TokenType.Colon);
            ExpectTypeToken();
            Next();
            if (CurrentToken.Type == TokenType.Comma)
            {
                Next();
                ParseParameter();
            }
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
            throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - Type expected - found {token.Type}");
        }
        
        private void Expect(TokenType tokenType)
        {
            var token = Next();
            if (token.Type != tokenType)
            {
                throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - expected {tokenType} - found {token.Type}");
            }
        }

        private void Assert(TokenType tokenType)
        {
            var token = CurrentToken;
            if (token.Type != tokenType)
            {
                throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - expected {tokenType} - found {token.Type}");
            }
        }

        private Token CurrentToken => _enumerator.Current ?? new Token(TokenType.Eof, string.Empty, 0, 0);

        private Token Next()
        {
            return _enumerator.MoveNext()
                ? _enumerator.Current
                : new Token(TokenType.Eof, string.Empty, 0, 0);
        }
    }
}