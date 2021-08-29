using System;
using System.Linq;
using Waddle.Core.Symbols;
using Waddle.Core.Syntax;
using Xunit;

namespace Waddle.Test
{
    public class ParserTest
    {
        [Fact]
        public void OneEmptyFunction()
        {
            var tokens = new[]
            {
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.LBrace, TokenType.RBrace
            };
            Parser p = new Parser(tokens.Select(x => new Token(x, "", 0, 0)));
            
            p.Parse();
        }
        
        [Fact]
        public void TwoEmptyFunctions()
        {
            var tokens = new[]
            {
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.LBrace, TokenType.RBrace,
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.LBrace, TokenType.RBrace
            };
            Parser p = new Parser(tokens.Select(x => new Token(x, "", 0, 0)));
            
            p.Parse();
        }
        
        [Fact]
        public void OneReturnFunction()
        {
            var tokens = new[]
            {
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Arrow, TokenType.Int, TokenType.LBrace, TokenType.Return, TokenType.Identifier, TokenType.RBrace
            };
            Parser p = new Parser(tokens.Select(x => new Token(x, "", 0, 0)));
            
            p.Parse();
        }
        
        [Fact]
        public void TwoReturnFunction()
        {
            var tokens = new[]
            {
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Arrow, TokenType.Int, TokenType.LBrace, TokenType.Return, TokenType.Identifier, TokenType.RBrace,
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Arrow, TokenType.Int, TokenType.LBrace, TokenType.Return, TokenType.Identifier, TokenType.RBrace
            };
            Parser p = new Parser(tokens.Select(x => new Token(x, "", 0, 0)));
            
            p.Parse();
        }
        
        
        [Fact]
        public void OneInvocationFunction()
        {
            var tokens = new[]
            {
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Arrow, TokenType.Int, TokenType.LBrace, TokenType.Arrow, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.RBrace
            };
            Parser p = new Parser(tokens.Select(x => new Token(x, "", 0, 0)));
            
            p.Parse();
        }
        
        [Fact]
        public void TwoInvocationFunction()
        {
            var tokens = new[]
            {
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Arrow, TokenType.Int, TokenType.LBrace, TokenType.Arrow, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.RBrace,
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Arrow, TokenType.Int, TokenType.LBrace, TokenType.Arrow, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.RBrace
            };
            Parser p = new Parser(tokens.Select(x => new Token(x, "", 0, 0)));
            
            p.Parse();
        }
    }
}
