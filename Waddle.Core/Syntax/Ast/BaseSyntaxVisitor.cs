using System.Collections.Generic;

namespace Waddle.Core.Syntax.Ast
{
    public class BaseSyntaxVisitor<TState, TResult> : ISyntaxVisitor<TState, TResult>
    {
        private readonly Stack<Syntax> _stack = new();

        protected BaseSyntaxVisitor(TResult defaultResult)
        {
            DefaultResult = defaultResult;
        }

        protected TResult DefaultResult { get; }

        /// <summary>
        /// Gets a collection containing all ancestors of the current syntax, beginning with the direct parent.
        /// </summary>
        protected IReadOnlyList<Syntax> Ancestors => _stack.ToArray();

        /// <summary>
        /// Gets the direct parent of the current syntax if any.
        /// </summary>
        protected Syntax? Parent => _stack.TryPeek(out var syntax) ? syntax : null;

        public virtual TResult Visit(ProgramSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            foreach (var function in syntax.FunctionDeclarations)
            {
                function.Accept(this, state);
            }

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(FunctionDeclSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            syntax.Body.Accept(this, state);

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(TypeSyntax syntax, TState state)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(ParameterDeclSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            syntax.TypeSyntax.Accept(this, state);

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(BlockSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            foreach (var stmt in syntax.Statements)
            {
                stmt.Accept(this, state);
            }

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(ReturnStmtSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            syntax.Expression.Accept(this, state);

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(IfStmtSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            syntax.Expression.Accept(this, state);
            syntax.Body.Accept(this, state);

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(AssignStmtSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            syntax.Expression.Accept(this, state);

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(PrintStmtSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            foreach (var argument in syntax.Arguments)
            {
                argument.Accept(this, state);
            }

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(DeclStmtSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            syntax.ParameterDeclSyntax.Accept(this, state);
            syntax.Expression.Accept(this, state);

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(InvocationStmtSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            syntax.Expression.Accept(this, state);

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(TermExpressionSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            syntax.Left.Accept(this, state);
            syntax.Right.Accept(this, state);

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(LogicalExpressionSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            syntax.Left.Accept(this, state);
            syntax.Right.Accept(this, state);

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(RelationalExpressionSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            syntax.Left.Accept(this, state);
            syntax.Right.Accept(this, state);

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(ProductExpressionSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            syntax.Left.Accept(this, state);
            syntax.Right.Accept(this, state);

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(InvocationExpressionSyntax syntax, TState state)
        {
            _stack.Push(syntax);

            foreach (var argument in syntax.Arguments)
            {
                argument.Accept(this, state);
            }

            _stack.Pop();
            return DefaultResult;
        }

        public virtual TResult Visit(IntegerLiteralAtom syntax, TState state)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(BoolLiteralAtom syntax, TState state)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(StringLiteralAtom syntax, TState state)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(IdentifierAtom syntax, TState state)
        {
            return DefaultResult;
        }
    }
}