using System.Collections.Generic;

namespace Waddle.Core.Syntax.Ast
{
    public class BaseSyntaxVisitor<TResult> : ISyntaxVisitor<TResult>
    {
        protected BaseSyntaxVisitor(TResult defaultResult)
        {
            DefaultResult = defaultResult;
        }

        protected TResult DefaultResult { get; }

        public virtual TResult Visit(ProgramSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(FunctionDeclSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(TypeSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(ParameterDeclSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(BlockSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(ReturnStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(IfStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(AssignStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(PrintStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(DeclStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(InvocationStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(TermExpressionSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(LogicalExpressionSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(RelationalExpressionSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(ProductExpressionSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(InvocationExpressionSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(IntegerLiteralAtom syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(BoolLiteralAtom syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(StringLiteralAtom syntax)
        {
            return DefaultResult;
        }

        public virtual TResult Visit(IdentifierAtom syntax)
        {
            return DefaultResult;
        }
    }
}