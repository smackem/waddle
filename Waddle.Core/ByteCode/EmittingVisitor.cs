using System;
using System.Collections.Generic;
using Waddle.Core.Ast;
using Waddle.Core.Semantics;
using Waddle.Core.Syntax;

namespace Waddle.Core.ByteCode
{
    public class EmittingVisitor : BaseSyntaxVisitor<TypeSymbol>
    {
        private readonly TypeExtractingVisitor _typeExtractor;
        private readonly List<Instruction> _instructions = new();

        public Instruction[] BuildProgram()
        {
            return _instructions.ToArray();
        }

        public EmittingVisitor(IReadOnlyDictionary<string, Symbol> symbols)
            : base(TypeSymbol.Void)
        {
            _typeExtractor = new TypeExtractingVisitor(symbols);
        }

        public override TypeSymbol VisitProgram(ProgramSyntax syntax)
        {
            foreach (var function in syntax.FunctionDeclarations)
            {
                if (function.Name == Naming.EntryPointFunctionName)
                {
                    function.Accept(this);
                }
            }

            return DefaultResult;
        }

        public override TypeSymbol VisitFunctionDecl(FunctionDeclSyntax syntax)
        {
            return syntax.Body.Accept(this);
        }

        public override TypeSymbol VisitBlock(BlockSyntax syntax)
        {
            foreach (var stmt in syntax.Statements)
            {
                stmt.Accept(this);
            }

            return DefaultResult;
        }

        public override TypeSymbol VisitReturnStmt(ReturnStmtSyntax syntax)
        {
            return syntax.Expression.Accept(this);
        }

        public override TypeSymbol VisitIntegerLiteral(IntegerLiteralAtom syntax)
        {
            Emit(OpCode.PushI32, syntax.Value);
            return TypeSymbol.Integer;
        }

        public override TypeSymbol VisitTermExpr(TermExpressionSyntax term)
        {
            term.Left.Accept(this);
            term.Right.Accept(this);

            var type = term.Accept(_typeExtractor);
            if (type != TypeSymbol.Integer)
            {
                throw new Exception("only int supported");
            }

            var opCode = term.Operator switch {
                TermOperator.Plus => OpCode.AddI32,
                TermOperator.Minus => OpCode.SubI32,
                _ => throw new ArgumentOutOfRangeException(),
            };

            Emit(opCode);
            return type;
        }

        private void Emit(OpCode opCode)
        {
            _instructions.Add(new Instruction(opCode));
        }

        private void Emit(OpCode opCode, int integerArg)
        {
            _instructions.Add(new Instruction(opCode) { IntegerArg = integerArg });
        }
    }
}
