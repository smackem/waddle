using System.Collections.Generic;
using Waddle.Core.Ast;

namespace Waddle.Core.ByteCode
{
/*

(1 + 2) * 5

[]
push 1

[ 1 ]

push 2

[ 1 2 ]

add

[ 3 ]

push 5

[ 3 5 ]

mul

[ 15 ]


0000 push 1
0001 branch 0

*/

    public class Instruction
    {
        public OpCode OpCode { get; }
        public int IntegerArg { get; set; }

        public Instruction(OpCode opCode)
        {
            OpCode = opCode;
        }

        public override string ToString()
        {
            return $"{nameof(OpCode)}: {OpCode}, {nameof(IntegerArg)}: {IntegerArg}";
        }
    }
}
