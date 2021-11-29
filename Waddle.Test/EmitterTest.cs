using System.Linq;
using Waddle.Core.Ast;
using Waddle.Core.ByteCode;
using Waddle.Core.Lexing;
using Waddle.Core.Syntax;
using Xunit;

namespace Waddle.Test
{
    public class EmitterTest
    {
        [Fact]
        public void Empty()
        {
            const string source = @"
                function main() {
                }";

            var program = LexParseAndEmit(source);
            Assert.Empty(program);
        }

        [Fact]
        public void Minimal()
        {
            const string source = @"
                function main() {
                    return 1 + 2;
                }";

            var program = LexParseAndEmit(source);
            Assert.Collection(program,
                instr =>
                {
                    Assert.Equal(OpCode.PushI32, instr.OpCode);
                    Assert.Equal(1, instr.IntegerArg);
                },
                instr =>
                {
                    Assert.Equal(OpCode.PushI32, instr.OpCode);
                    Assert.Equal(2, instr.IntegerArg);
                },
                instr =>
                {
                    Assert.Equal(OpCode.AddI32, instr.OpCode);
                });
        }

        private static Instruction[] LexParseAndEmit(string source)
        {
            var reader = source.CharwiseWithTrimmedLines();
            using var lexer = new Lexer(reader);
            var tokens = lexer.Lex().ToList();
            var parser = new Parser(tokens);
            var ast = (ProgramSyntax) parser.Parse();
            var functions = new SymbolWaddler().WaddleProgram(ast);
            var emitter = new EmittingVisitor(functions[Naming.EntryPointFunctionName].Variables
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value as Symbol));
            ast.Accept(emitter);
            return emitter.BuildProgram();
        }
    }
}