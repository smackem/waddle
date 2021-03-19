using System;
using System.IO;
using Waddle.Core.Symbols;
using Xunit;

namespace Waddle.Test {
    public class LexerTest {
        [Fact]
        public void Numbers() {
            using var reader = new CharReader(new StringReader(
                @"abc\n123 blah\n!@$+"));
            using var lexer = new Lexer(reader);
            Assert.Collection(lexer.Lex(),
    t => Assert.Equal(TokenType.Number, t.Type));
        }
    }
}