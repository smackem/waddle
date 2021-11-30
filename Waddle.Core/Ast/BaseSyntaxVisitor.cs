namespace Waddle.Core.Ast
{
    public class BaseSyntaxVisitor<TResult> : ISyntaxVisitor<TResult>
    {
        protected BaseSyntaxVisitor(TResult defaultResult)
        {
            DefaultResult = defaultResult;
        }

        protected TResult DefaultResult { get; }

        public virtual TResult VisitProgram(ProgramSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitFunctionDecl(FunctionDeclSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitType(TypeSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitParameterDecl(ParameterDeclSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitBlock(BlockSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitReturnStmt(ReturnStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitIfStmt(IfStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitAssignStmt(AssignStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitPrintStmt(PrintStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitDeclStmt(DeclStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitInvocationStmt(InvocationStmtSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitTermExpr(TermExpressionSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitLogicalExpr(LogicalExpressionSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitRelationalExpr(RelationalExpressionSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitProductExpr(ProductExpressionSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitInvocationExpr(InvocationExpressionSyntax syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitIntegerLiteral(IntegerLiteralAtom syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitBoolLiteral(BoolLiteralAtom syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitStringLiteral(StringLiteralAtom syntax)
        {
            return DefaultResult;
        }

        public virtual TResult VisitIdentifier(IdentifierAtom syntax)
        {
            return DefaultResult;
        }
    }
}