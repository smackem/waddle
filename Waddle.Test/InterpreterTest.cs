using Waddle.Core.ByteCode;
using Waddle.Core.Runtime;
using Xunit;

namespace Waddle.Test
{
    public class InterpreterTest
    {
        [Fact]
        public void AddOneAndTwo()
        {
            var program = new Instruction[]
            {
                new(OpCode.PushI32) { IntegerArg = 1 },
                new(OpCode.PushI32) { IntegerArg = 2 },
                new(OpCode.AddI32),
            };
            var interpreter = new Interpreter();
            var stack = new RuntimeStack();
            interpreter.Interpret(program, stack);

            Assert.Equal(1, stack.Count);
            Assert.Equal(3, stack.Get(0).Integer);
        }

        [Fact]
        public void Loop()
        {
            /*
             * 00 push 0 -> 0 = count
             * 01 push 0 -> 1 = sum
             * 02 push 0 -> 2 = n
             * 
             * n = 1
             * 03 push 1
             * 04 store 2
             * 
             * sum = sum + n
             * 05 load 1
             * 06 load 2
             * 07 add
             * 08 store 1
             * 
             * n = n + 1
             * 09 load 2
             * 10 push 1
             * 11 add
             * 12 store 2
             *
             * if n != 5 then goto :loop
             * 13 load 2
             * 14 push 5
             * 15 Eq
             * 16 Branch_zero 05
             */

            var program = new Instruction[]
            {
                new(OpCode.PushI32) { IntegerArg = 1 },
                new(OpCode.PushI32) { IntegerArg = 2 },
                new(OpCode.AddI32),
            };
            var interpreter = new Interpreter();
            var stack = new RuntimeStack();
            interpreter.Interpret(program, stack);

            Assert.Equal(1, stack.Count);
            Assert.Equal(3, stack.Get(0).Integer);
        }
    }
}