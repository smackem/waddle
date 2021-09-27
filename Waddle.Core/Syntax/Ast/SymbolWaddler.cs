using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace Waddle.Core.Syntax.Ast
{
    public class SymbolWaddler
    {
        private readonly IDictionary<string, FunctionDecl> _functions =
            new Dictionary<string, FunctionDecl>();

        public IImmutableDictionary<string, FunctionDecl> Functions => _functions.ToImmutableDictionary();

        public void WaddleProgram(ProgramSyntax program)
        {
            foreach (var function in program.FunctionDeclarations)
            {
                WaddleFunctionDeclaration(function);
            }
        }

        private void WaddleFunctionDeclaration(FunctionDeclSyntax functionSyntax)
        {
            var function = new FunctionDecl(functionSyntax.Name, functionSyntax.ReturnType,
                functionSyntax.Body.Statements
                    .Select(stmt => stmt as DeclStmtSyntax)
                    .Where(stmt => stmt != null)
                    .Select(stmt => new VariableDecl(stmt!.ParameterDecSyntax.Name, stmt.ParameterDecSyntax.TypeSyntax)));
            _functions.Add(function.Name, function);
        }
    }
}