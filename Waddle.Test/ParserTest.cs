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
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Arrow, TokenType.Int, TokenType.LBrace, TokenType.Arrow, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Semicolon, TokenType.RBrace
            };
            Parser p = new Parser(tokens.Select(x => new Token(x, "", 0, 0)));
            
            p.Parse();
        }
        
        [Fact]
        public void TwoInvocationFunction()
        {
            var tokens = new[]
            {
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Arrow, TokenType.Int, TokenType.LBrace, TokenType.Arrow, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Semicolon, TokenType.RBrace,
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Arrow, TokenType.Int, TokenType.LBrace, TokenType.Arrow, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Semicolon, TokenType.RBrace
            };
            Parser p = new Parser(tokens.Select(x => new Token(x, "", 0, 0)));
            
            p.Parse();
        }

        [Fact]
        public void IntegrationTest_Step1()
        {
            var reader = @"
                function addTwoNumbers(a: int, b: int) -> int {
                    return a + b;
                }

                function getGreaterNumber(a: int, b: int) -> int {
                    if a > b {
                        return a;
                    }
                    return b;
                }

                function main() {
                    var x: int = ->addTwoNumbers(1, 2);
                    var y: int = ->getGreaterNumber(x, 100);
                    print(x, y);
                }
                ".CharwiseWithTrimmedLines();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            var parser = new Parser(tokens);
            parser.Parse();
        }
    }
}
