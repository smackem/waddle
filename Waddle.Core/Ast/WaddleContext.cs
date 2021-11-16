using System.Collections.Generic;

namespace Waddle.Core.Ast
{
    public class WaddleContext
    {
        private readonly Stack<Syntax> _ancestors;
        
        internal WaddleContext(Stack<Syntax> ancestors)
        {
            _ancestors = ancestors;
        }

        /// <summary>
        /// Gets a collection containing all ancestors of the current syntax, beginning with the direct parent.
        /// </summary>
        protected IReadOnlyList<Syntax> Ancestors => _ancestors.ToArray();

        /// <summary>
        /// Gets the direct parent of the current syntax if any.
        /// </summary>
        public Syntax? Parent
        {
            get
            {
                _ancestors.TryPop(out var syntaxCurrent);
                _ancestors.TryPeek(out var syntax);
                if (syntaxCurrent != null)
                {
                    _ancestors.Push(syntaxCurrent);
                }
                
                return syntax;
            }
        }
    }
}