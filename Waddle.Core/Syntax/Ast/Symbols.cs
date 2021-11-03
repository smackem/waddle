using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace Waddle.Core.Syntax.Ast
{
    public abstract class Symbol
    {
        public string Name { get; }
        public TypeSymbol? Type { get; }

        public Symbol(string name, TypeSymbol? type)
        {
            Name = name;
            Type = type;
        }
    }

    public class FunctionDecl : Symbol
    {
        private readonly IDictionary<string, VariableDecl> _variables;

        public IImmutableDictionary<string, VariableDecl> Variables => _variables.ToImmutableDictionary();
        public IReadOnlyList<VariableDecl> Parameters { get; }

        public FunctionDecl(string name, TypeSymbol? type, IEnumerable<VariableDecl> variables, IEnumerable<VariableDecl> parameters)
            : base(name, type)
        {
            _variables = variables.ToDictionary(v => v.Name);
            Parameters = new ReadOnlyCollection<VariableDecl>(parameters.ToArray());
        }
    }

    public class VariableDecl : Symbol
    {
        public VariableDecl(string name, TypeSymbol type) : base(name, type)
        {
        }
    }

    public class TypeSymbol : Symbol
    {
        private TypeSymbol(string name) : base(name, null)
        {
        }

        public static readonly TypeSymbol Integer = new TypeSymbol("int");
        public static readonly TypeSymbol String = new TypeSymbol("string");
        public static readonly TypeSymbol Bool = new TypeSymbol("bool");
        public static readonly TypeSymbol Void = new TypeSymbol("void");

        public override string ToString()
        {
            return this.Name;
        }
    }
}
