using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Waddle.Core.Syntax.Ast
{
    public class TypeExtractingVisitor : BaseSyntaxVisitor<TypeSymbol>
    {
        private readonly IDictionary<string, Symbol> _symbols;
        
        public TypeExtractingVisitor(IDictionary<string, Symbol> symbols) : base(TypeSymbol.Void)
        {
            _symbols = symbols;
        }

        public override TypeSymbol Visit(LogicalExpressionSyntax syntax)
        {
            return TypeSymbol.Bool;
        }

        public override TypeSymbol Visit(RelationalExpressionSyntax syntax)
        {
            return TypeSymbol.Bool;
        }

        public override TypeSymbol Visit(StringLiteralAtom syntax)
        {
            return TypeSymbol.String;
        }

        public override TypeSymbol Visit(TermExpressionSyntax syntax)
        {
            return GetTermType(syntax.Left.Accept(this), syntax.Right.Accept(this));
        }

        public override TypeSymbol Visit(IdentifierAtom syntax)
        {
            return _symbols[syntax.Identifier].Type!;
        }

        public override TypeSymbol Visit(BoolLiteralAtom syntax)
        {
            return TypeSymbol.Bool;
        }

        public override TypeSymbol Visit(IntegerLiteralAtom syntax)
        {
            return TypeSymbol.Integer;
        }

        private static TypeSymbol GetTermType(TypeSymbol left, TypeSymbol right)
        {
            return (left, right) switch
            {
                var (l, r) when l == TypeSymbol.Bool && r == TypeSymbol.Bool => TypeSymbol.Bool,
                var (l, r) when l == TypeSymbol.String && r == TypeSymbol.Integer => TypeSymbol.String,
                var (l, r) when l == TypeSymbol.String && r == TypeSymbol.Bool => TypeSymbol.String,
                var (l, r) when l == TypeSymbol.Integer && r == TypeSymbol.String => TypeSymbol.String,
                var (l, r) when l == TypeSymbol.Bool && r == TypeSymbol.String => TypeSymbol.String,
                _ => left,
            };
        }
    }
}