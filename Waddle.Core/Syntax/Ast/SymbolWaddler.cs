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

        public IImmutableDictionary<string, FunctionDecl> WaddleProgram(ProgramSyntax program)
        {
            foreach (var function in program.FunctionDeclarations)
            {
                WaddleFunctionDeclaration(function);
            }

            return _functions.ToImmutableDictionary();
        }

        private void WaddleFunctionDeclaration(FunctionDeclSyntax functionSyntax)
        {
            var function = new FunctionDecl(functionSyntax.Name, functionSyntax.ReturnType?.ToSymbol(),
                functionSyntax.Body.Statements
                    .Select(stmt => stmt as DeclStmtSyntax)
                    .Where(stmt => stmt != null)
                    .Select(stmt => new VariableDecl(stmt!.ParameterDeclSyntax.Name, stmt.ParameterDeclSyntax.TypeSyntax.ToSymbol())));
            _functions.Add(function.Name, function);
        }
    }
}