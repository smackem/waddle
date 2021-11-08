using System;

namespace Waddle.Core.Syntax.Ast
{
    // TODO: add line and char position
    public class SemanticErrorException : Exception
    {
        public SemanticErrorException(string message) : base(message)
        {
        }
    }
}