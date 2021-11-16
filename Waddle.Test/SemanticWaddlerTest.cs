using System.Collections.Immutable;
using System.Linq;
using Waddle.Core.Ast;
using Waddle.Core.Lexing;
using Waddle.Core.Semantics;
using Waddle.Core.Syntax;
using Xunit;

namespace Waddle.Test
{
    public class SemanticWaddlerTest
    {
        [Fact]
        public void ThrowsOnIncompatibleTypesInDeclaration()
        {
            const string source = @"
                function main() {
                    var n: int = false;
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnIncompatibleTypesInAssignment()
        {
            const string source = @"
                function main() {
                    var n: int = 0;
                    n = true;
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnUnkownVariable()
        {
            const string source = @"
                function main() {
                    number666 = 0;
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnUnkownFunction()
        {
            const string source = @"
                function main() {
                    ->nonExistingFunction();
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnTooManyFunctionArguments()
        {
            const string source = @"
                function f() {
                }

                function main() {
                    ->f(1);
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnTooFewFunctionArguments()
        {
            const string source = @"
                function f(x: int) {
                }

                function main() {
                    ->f();
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnTypeMismatchedFunctionArgument()
        {
            const string source = @"
                function f(x: int) {
                }

                function main() {
                    ->f(true);
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnTypeMismatchedFunctionArgument2()
        {
            const string source = @"
                function f(a: int, b: int, c: bool, d: int) {
                }

                function main() {
                    ->f(1, 2, true, false);
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnTypeMismatchedFunctionInvocation()
        {
            const string source = @"
                function f() -> bool {
                    return false;
                }

                function main() {
                    var r: int = ->f();
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnMissingReturnStmt()
        {
            const string source = @"
                function f() -> bool {
                    // no return stmt
                }

                function main() {
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnReturnTypeMismatch()
        {
            const string source = @"
                function f() -> bool {
                    // no return stmt
                }

                function main() {
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsErrorBecauseOfInvocationWithWrongType()
        {
            const string source = @"
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
                ";

            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }
        
        [Fact]
        public void ThrowsOnMissingEntryPoint()
        {
            const string source = @"
                function f() -> bool {
                    return false;
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }
 
        [Fact]
        public void ThrowsOnIncompatibleTypesInComparisonEq()
        {
            const string source = @"
                function main() {
                    if 1 == false {
                    }
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }
 
        [Fact]
        public void ThrowsOnIncompatibleTypesInComparisonNe()
        {
            const string source = @"
                function main() {
                    if 1 != false {
                    }
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }
 
        [Fact]
        public void ThrowsOnIncompatibleTypesInComparisonLt()
        {
            const string source = @"
                function main() {
                    if true > false {
                    }
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }
 
        [Fact]
        public void ThrowsOnIncompatibleTypesInAddition()
        {
            const string source = @"
                function main() {
                    var x: int = true + 1;
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }
 
        [Fact]
        public void ThrowsOnIncompatibleTypesInProduct()
        {
            const string source = @"
                function main() {
                    var x: int = 12 * false;
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnEntryPointSignatureMismatch1()
        {
            const string source = @"
                function main(illegalParameter: bool) {
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnEntryPointSignatureMismatch2()
        {
            const string source = @"
                function main() -> bool {
                    return false;
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnIfStmtWithoutConditionalExpr()
        {
            const string source = @"
                function main() {
                    if 1 { }
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        [Fact]
        public void ThrowsOnNestedDeclStmt()
        {
            const string source = @"
                function main() {
                    if true {
                        var illegal: int = 0;
                    }
                }
                ";
            var (ast, functions) = LexParseAndWaddleSymbols(source);
            Assert.Throws<SemanticErrorException>(() => new SemanticWaddler(functions).WaddleProgram(ast));
        }

        private static (ProgramSyntax ast, IImmutableDictionary<string, FunctionDecl> functions) LexParseAndWaddleSymbols(
            string source)
        {
            var reader = source.CharwiseWithTrimmedLines();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            var parser = new Parser(tokens);
            var ast = (ProgramSyntax) parser.Parse();
            var functions = new SymbolWaddler().WaddleProgram(ast);
            return (ast, functions);
        }
    }
}