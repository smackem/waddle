using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Waddle.Core.Syntax.Ast
{
    public abstract class Symbol
    {
        public string Name { get; }
        public TypeSyntax? Type { get; }

        public Symbol(string name, TypeSyntax? type)
        {
            Name = name;
            Type = type;
        }
    }

    public class FunctionDecl : Symbol
    {
        private readonly IDictionary<string, VariableDecl> _variables;

        public IImmutableDictionary<string, VariableDecl> Variables => _variables.ToImmutableDictionary();

        public FunctionDecl(string name, TypeSyntax type, IEnumerable<VariableDecl> variables)
            : base(name, type)
        {
            _variables = variables.ToDictionary(v => v.Name);
        }
    }

    public class VariableDecl : Symbol
    {
        public VariableDecl(string name, TypeSyntax type) : base(name, type)
        {
        }
    }
}
