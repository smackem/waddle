using System;

namespace Waddle.Core.Semantics
{
    // TODO: add line and char position
    public class SemanticErrorException : Exception
    {
        public SemanticErrorException(string message) : base(message)
        {
        }
    }
}