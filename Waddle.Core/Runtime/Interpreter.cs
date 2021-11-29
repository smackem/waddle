using System;
using Waddle.Core.ByteCode;

namespace Waddle.Core.Runtime
{
    public class Interpreter
    {
        public void Interpret(Instruction[] program, RuntimeStack stack)
        {
            int pc = 0;
            RuntimeValue left, right;
            for ( ; pc < program.Length; pc++)
            {
                var instr = program[pc];
                switch (instr.OpCode)
                {
                    case OpCode.PushI32:
                        stack.Push(new RuntimeValue(instr.IntegerArg));
                        break;
                    case OpCode.AddI32:
                        (right, left) = (stack.Pop(), stack.Pop());
                        stack.Push(new RuntimeValue(left.Integer + right.Integer));
                        break;
                    case OpCode.StoreLocalI32:
                        stack[instr.IntegerArg] = stack.Pop();
                        break;
                    case OpCode.LoadLocalI32:
                        stack.Push(new RuntimeValue(stack[instr.IntegerArg].Integer));
                        break;
                    case OpCode.Branch:
                        pc = instr.IntegerArg - 1;
                        break;
                    case OpCode.EqI32:
                        (right, left) = (stack.Pop(), stack.Pop());
                        stack.Push(right.Integer == left.Integer ? new RuntimeValue(1) : new RuntimeValue(0));
                        break;
                    case OpCode.BranchZero:
                        left = stack.Pop();
                        if (left.Integer == 0)
                        {
                            pc = instr.IntegerArg - 1;
                        }
                        break; 
                    default:
                        throw new ArgumentOutOfRangeException($"unknown opcode {instr.OpCode}");
                }
            }
        }
    }
}