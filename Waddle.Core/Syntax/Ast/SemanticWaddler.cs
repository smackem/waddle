using System;
using System.Collections.Generic;

namespace Waddle.Core.Syntax.Ast
{
    public class SemanticWaddler
    {
        private readonly IDictionary<string, FunctionDeclSyntax> _functions =
            new Dictionary<string, FunctionDeclSyntax>();

        public void WaddleProgram(ProgramSyntax program)
        {
            var hasEntryPoint = false;

            foreach (var function in program.FunctionDeclarations)
            {
                if (function.Name == Naming.EntryPointFunctionName)
                {
                    hasEntryPoint = true;
                }
                WaddleFunctionDeclaration(function);
            }

            if (hasEntryPoint == false)
            {
                throw new SemanticErrorException();
            }
        }

        private void WaddleFunctionDeclaration(FunctionDeclSyntax function)
        {
            if (_functions.ContainsKey(function.Name))
            {
                throw new SemanticErrorException();
            }

            _functions.Add(function.Name, function);
        }
    }
}