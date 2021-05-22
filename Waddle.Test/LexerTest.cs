using System;
using System.IO;
using System.Linq;
using Waddle.Core.Symbols;
using Xunit;

namespace Waddle.Test {
    public class LexerTest {
        [Fact]
        public void Numbers() {
            using var reader = new CharReader(new StringReader(
                "abc\n123 blah\n!@$+"));
            using var lexer = new Lexer(reader);
            foreach (var token in lexer.Lex())
            {
                Assert.True(token.Type is TokenType.Number or TokenType.Identifier or TokenType.Plus);
            }
        }
        
        [Fact]
        public void LexerTest123() {
            using var reader = new CharReader(new StringReader(
                @"// `let` declares a binding which cannot be assigned to.

function addTwoNumbers(a: int, b: int) -> int {
    return a + b;
}

function getGreaterNumber(a: int, b: int) -> int {
    if (a > b) {
        return a;
    }
    return b;
}

function main() {
    var x: int = addTwoNumbers(1, 2);
    var y: int = getGreaterNumber(x, 100);
    print(x, y);
}"));
            using var lexer = new Lexer(reader);
            var sut = lexer.Lex().ToList();
            Assert.NotEmpty(sut);
        }
    }
}