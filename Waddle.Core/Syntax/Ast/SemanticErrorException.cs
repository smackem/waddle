using System;

namespace Waddle.Core.Syntax.Ast
{
    public class SemanticErrorException : Exception
    {
        public SemanticErrorException(string message) : base(message)
        {
        }
    }
}