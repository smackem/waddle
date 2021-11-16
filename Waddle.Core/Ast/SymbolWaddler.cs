using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Waddle.Core.Ast
{
    // TODO: translate into a IWaddleListener
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
            var parameters =
                functionSyntax.Parameters.Select(param => new VariableDecl(param!.Name, param!.TypeSyntax.ToSymbol())).ToArray();
            var function = new FunctionDecl(functionSyntax.Name, functionSyntax.ReturnType?.ToSymbol(),
                functionSyntax.Body.Statements
                    .OfType<DeclStmtSyntax>()
                    .Select(stmt => new VariableDecl(stmt.ParameterDeclSyntax.Name, stmt.ParameterDeclSyntax.TypeSyntax.ToSymbol()))
                    .Concat(parameters),
                parameters);
            _functions.Add(function.Name, function);
        }
    }
}