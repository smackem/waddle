using System.Collections.Generic;

namespace Waddle.Core.Syntax.Ast
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
        public Syntax? Parent => _ancestors.TryPeek(out var syntax) ? syntax : null;
    }
}