using System.Collections.Generic;
using Waddle.Core.Ast;

namespace Waddle.Core.Semantics
{
    public class TypeExtractingVisitor : BaseSyntaxVisitor<TypeSymbol>
    {
        private readonly IReadOnlyDictionary<string, Symbol> _symbols;
        
        public TypeExtractingVisitor(IReadOnlyDictionary<string, Symbol> symbols) : base(TypeSymbol.Void)
        {
            _symbols = symbols;
        }

        public override TypeSymbol VisitLogicalExpr(LogicalExpressionSyntax syntax)
        {
            return TypeSymbol.Bool;
        }

        public override TypeSymbol VisitRelationalExpr(RelationalExpressionSyntax syntax)
        {
            return TypeSymbol.Bool;
        }

        public override TypeSymbol VisitStringLiteral(StringLiteralAtom syntax)
        {
            return TypeSymbol.String;
        }

        public override TypeSymbol VisitTermExpr(TermExpressionSyntax syntax)
        {
            return GetTermType(syntax.Left.Accept(this), syntax.Right.Accept(this));
        }

        public override TypeSymbol VisitIdentifier(IdentifierAtom syntax)
        {
            return _symbols[syntax.Identifier].Type!;
        }

        public override TypeSymbol VisitBoolLiteral(BoolLiteralAtom syntax)
        {
            return TypeSymbol.Bool;
        }

        public override TypeSymbol VisitIntegerLiteral(IntegerLiteralAtom syntax)
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