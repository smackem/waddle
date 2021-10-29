using System.Collections.Generic;
using System.Collections.Immutable;
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

        public FunctionDecl(string name, TypeSymbol? type, IEnumerable<VariableDecl> variables)
            : base(name, type)
        {
            _variables = variables.ToDictionary(v => v.Name);
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
    }
}
