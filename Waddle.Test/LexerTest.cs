using System;
using System.Linq;
using Waddle.Core.Symbols;
using Xunit;

namespace Waddle.Test
{
    public class LexerTest
    {
        [Fact]
        public void Empty()
        {
            using var reader = string.Empty.Charwise();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            Assert.Empty(tokens);
        }

        [Fact]
        public void Whitespace()
        {
            using var reader = " \n\t  \r".Charwise();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            Assert.Empty(tokens);
        }

        [Fact]
        public void Numbers()
        {
            using var reader = @"123 456 789".Charwise();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            Assert.Collection(tokens,
                t => AssertToken(t, TokenType.Number, "123"),
                t => AssertToken(t, TokenType.Number, "456"),
                t => AssertToken(t, TokenType.Number, "789"));
        }

        [Fact]
        public void Identifiers()
        {
            using var reader = @"Tod wo ist Dein Stachel".Charwise();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            Assert.Collection(tokens,
                t => AssertToken(t, TokenType.Identifier, "Tod"),
                t => AssertToken(t, TokenType.Identifier, "wo"),
                t => AssertToken(t, TokenType.Identifier, "ist"),
                t => AssertToken(t, TokenType.Identifier, "Dein"),
                t => AssertToken(t, TokenType.Identifier, "Stachel"));
        }

        [Fact]
        public void Keywords()
        {
            using var reader = @"function var if return int print for".Charwise();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            Assert.Collection(tokens,
                t => AssertToken(t, TokenType.Function),
                t => AssertToken(t, TokenType.Var),
                t => AssertToken(t, TokenType.If),
                t => AssertToken(t, TokenType.Return),
                t => AssertToken(t, TokenType.Int),
                t => AssertToken(t, TokenType.Print),
                t => AssertToken(t, TokenType.For));
        }
        
        [Fact]
        public void CaseSensitivity()
        {
            using var reader = @"Function function Var var".Charwise();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            Assert.Collection(tokens,
                t => AssertToken(t, TokenType.Identifier, "Function"),
                t => AssertToken(t, TokenType.Function),
                t => AssertToken(t, TokenType.Identifier, "Var"),
                t => AssertToken(t, TokenType.Var));
        }

        [Fact]
        public void Operators()
        {
            using var reader = @"+ - * / = == < > <= >= -> & |".Charwise();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            Assert.Collection(tokens,
                t => AssertToken(t, TokenType.Plus),
                t => AssertToken(t, TokenType.Minus),
                t => AssertToken(t, TokenType.Multiply),
                t => AssertToken(t, TokenType.Divide),
                t => AssertToken(t, TokenType.Equal),
                t => AssertToken(t, TokenType.Equals),
                t => AssertToken(t, TokenType.LessThan),
                t => AssertToken(t, TokenType.GreaterThan),
                t => AssertToken(t, TokenType.LessEquals),
                t => AssertToken(t, TokenType.GreaterEquals),
                t => AssertToken(t, TokenType.Arrow),
                t => AssertToken(t, TokenType.And),
                t => AssertToken(t, TokenType.Or));
        }

        [Fact]
        public void CompleteFunction()
        {
            using var reader = @"
                function getGreaterNumber(a: int, b: int) -> int {
                    var x: int = 100;
                    if a > b {
                        return a;
                    }
                    return b + x;
                }
                ".CharwiseWithTrimmedLines();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            Assert.Collection(tokens,
                t => AssertToken(t, TokenType.Function),
                t => AssertToken(t, TokenType.Identifier, "getGreaterNumber"),
                t => AssertToken(t, TokenType.LParen),
                t => AssertToken(t, TokenType.Identifier, "a"),
                t => AssertToken(t, TokenType.Colon),
                t => AssertToken(t, TokenType.Int),
                t => AssertToken(t, TokenType.Comma),
                t => AssertToken(t, TokenType.Identifier, "b"),
                t => AssertToken(t, TokenType.Colon),
                t => AssertToken(t, TokenType.Int),
                t => AssertToken(t, TokenType.RParen),
                t => AssertToken(t, TokenType.Arrow),
                t => AssertToken(t, TokenType.Int),
                t => AssertToken(t, TokenType.LBrace),
                t => AssertToken(t, TokenType.Var),
                t => AssertToken(t, TokenType.Identifier, "x"),
                t => AssertToken(t, TokenType.Colon),
                t => AssertToken(t, TokenType.Int),
                t => AssertToken(t, TokenType.Equal),
                t => AssertToken(t, TokenType.Number, "100"),
                t => AssertToken(t, TokenType.Semicolon),
                t => AssertToken(t, TokenType.If),
                t => AssertToken(t, TokenType.Identifier, "a"),
                t => AssertToken(t, TokenType.GreaterThan),
                t => AssertToken(t, TokenType.Identifier, "b"),
                t => AssertToken(t, TokenType.LBrace),
                t => AssertToken(t, TokenType.Return),
                t => AssertToken(t, TokenType.Identifier, "a"),
                t => AssertToken(t, TokenType.Semicolon),
                t => AssertToken(t, TokenType.RBrace),
                t => AssertToken(t, TokenType.Return),
                t => AssertToken(t, TokenType.Identifier, "b"),
                t => AssertToken(t, TokenType.Plus),
                t => AssertToken(t, TokenType.Identifier, "x"),
                t => AssertToken(t, TokenType.Semicolon),
                t => AssertToken(t, TokenType.RBrace));
        }

        [Fact]
        public void Comments()
        {
            using var reader = @"
                // this is a comment

                // another comment
                //= this is also a comment but it looks like a scary operator
                // this comment is followed by EOF, not by a line break"
                .CharwiseWithTrimmedLines();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            Assert.Empty(tokens);
        }

        [Fact]
        public void ThrowOnUnrecognizedInput()
        {
            using var reader = @"
                function addTwoNumbers(#unrecognized#) => int {
                    return a @ b;
                }
                ".CharwiseWithTrimmedLines();
            using var lexer = new Lexer(reader);
            Assert.Throws<Exception>(() =>
            {
                _ = lexer.Lex().ToList();
            });
        }

        private static void AssertToken(Token token, TokenType expectedType, string? expectedLexeme = null)
        {
            Assert.Equal(expectedType, token.Type);
            if (expectedLexeme != null)
            {
                Assert.Equal(expectedLexeme, token.Lexeme);
            }
        }
    }
}
