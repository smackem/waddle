using System;
using Waddle.Core.Lexing;

namespace Waddle.Core.Syntax.Ast
{
    public static class SyntaxExtensions
    {
        public static TypeSymbol ToSymbol(this TypeSyntax typeSyntax)
        {
            return typeSyntax.TypeToken.Type switch
            {
                TokenType.Int => TypeSymbol.Integer,
                TokenType.String => TypeSymbol.String,
                TokenType.Bool => TypeSymbol.Bool,
                var t => throw new ArgumentOutOfRangeException($"{t}"),
            };
        }
    }
}