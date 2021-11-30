using System.Linq;
using Waddle.Core.Ast;
using Waddle.Core.ByteCode;
using Waddle.Core.Lexing;
using Waddle.Core.Runtime;
using Waddle.Core.Semantics;
using Waddle.Core.Syntax;
using Xunit;

namespace Waddle.Test
{
    public class IntegrationTest
    {
        [Fact]
        public void Minimal()
        {
            const string source = @"
                function main() {
                    return 1 + 2;
                }";

            var stack = LexParseEmitAndInterpret(source);
            Assert.Equal(new RuntimeValue(3), stack.Pop());
        }

        private static RuntimeStack LexParseEmitAndInterpret(string source)
        {
            var reader = source.CharwiseWithTrimmedLines();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            var parser = new Parser(tokens);
            var ast = (ProgramSyntax) parser.Parse();
            var functions = new SymbolWaddler().WaddleProgram(ast);
            new Waddler(new SemanticWaddleListener(functions)).WaddleProgram(ast);
            var emitter = new EmittingVisitor(functions[Naming.EntryPointFunctionName].Variables
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value as Symbol));
            ast.Accept(emitter);
            var program = emitter.BuildProgram();
            var interpreter = new Interpreter();
            var stack = new RuntimeStack();
            interpreter.Interpret(program, stack);
            return stack;
        }
    }
}