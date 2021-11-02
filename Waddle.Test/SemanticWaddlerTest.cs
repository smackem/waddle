using System.Linq;
using Waddle.Core.Symbols;
using Waddle.Core.Syntax;
using Waddle.Core.Syntax.Ast;
using Xunit;

namespace Waddle.Test
{
    public class SemanticWaddlerTest
    {
        [Fact]
        public void ThrowsOnIncompatibleTypesInDeclaration()
        {
            var reader = @"
                function main() {
                    var n: int = false;
                }
                ".CharwiseWithTrimmedLines();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            var symbols = new SymbolWaddler().WaddleProgram((ProgramSyntax)ast);
            var semanticWaddler = new SemanticWaddler(symbols);
            Assert.Throws<SemanticErrorException>(() => semanticWaddler.WaddleProgram((ProgramSyntax)ast));
        }

        [Fact]
        public void ThrowsErrorBecauseOfInvocationWithWrongType()
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
                    //Here is the Error
                    var y: int = ->getGreaterNumber(x, true);
                    print(x, y);
                }
                ".CharwiseWithTrimmedLines();
            
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            var symbols = new SymbolWaddler().WaddleProgram((ProgramSyntax)ast);
            var semanticWaddler = new SemanticWaddler(symbols);
            Assert.Throws<SemanticErrorException>(() => semanticWaddler.WaddleProgram((ProgramSyntax)ast));
        }
    }
}