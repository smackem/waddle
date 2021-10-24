using System;
using System.Linq;
using Waddle.Core.Symbols;
using Waddle.Core.Syntax;
using Waddle.Core.Syntax.Ast;
using Xunit;

namespace Waddle.Test
{
    public class ParserTest
    {
        public void OneEmptyFunction()
        {
            var tokens = new[]
            {
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.LBrace, TokenType.RBrace
            };
            Parser p = new Parser(tokens.Select(x => new Token(x, "", 0, 0)));
            
            p.Parse();
        }
        
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
        
        public void OneReturnFunction()
        {
            var tokens = new[]
            {
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Arrow, TokenType.Int, TokenType.LBrace, TokenType.Return, TokenType.Identifier, TokenType.RBrace
            };
            Parser p = new Parser(tokens.Select(x => new Token(x, "", 0, 0)));
            
            p.Parse();
        }
        
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
        
        public void OneInvocationFunction()
        {
            var tokens = new[]
            {
                TokenType.Function, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Arrow, TokenType.Int, TokenType.LBrace, TokenType.Arrow, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.Semicolon, TokenType.RBrace
            };
            Parser p = new Parser(tokens.Select(x => new Token(x, "", 0, 0)));
            
            p.Parse();
        }
        
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
        public void ParserTest_Step1()
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

        [Fact]
        public void AstEmissionTest()
        {
            var reader = @"
                function main() {
                }
                ".CharwiseWithTrimmedLines();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            var symbols = new SymbolWaddler().WaddleProgram((ProgramSyntax)ast);
            var semanticWaddler = new SemanticWaddler(symbols);
            semanticWaddler.WaddleProgram((ProgramSyntax)ast);
        }
    }
}
