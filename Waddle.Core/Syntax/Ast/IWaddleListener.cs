using System;
using System.Collections.Generic;

namespace Waddle.Core.Syntax.Ast
{
    public interface IWaddleListener<T>
    {
        T OnProgram(ProgramSyntax syntax);
        void OnFunctionDeclaration(FunctionDeclSyntax syntax);
        T OnPrintStmt(PrintStmtSyntax syntax, IEnumerable<T> allArguments);
        T OnIdentifierLiteral(IdentifierAtom atom);
        T OnBoolLiteral(BoolLiteralAtom atom);
        T OnIntegerLiteral(IntegerLiteralAtom atom);
        T OnStringLiteral(StringLiteralAtom atom);
        T OnTermExpr(TermExpressionSyntax termExpr, Func<T> waddleExpressionLeft, Func<T> waddleExpressionRight);
        T OnProductExpr(ProductExpressionSyntax productExpr, Func<T> waddleExpressionLeft, Func<T> waddleExpressionRight);
        T OnRelationalExpr(RelationalExpressionSyntax relationalExpr, Func<T> waddleExpressionLeft, Func<T> waddleExpressionRight);
        T OnLogicalExpr(LogicalExpressionSyntax logicalExpr, Func<T> waddleExpressionLeft, Func<T> waddleExpressionRight);
        T OnInvocationExpr(InvocationExpressionSyntax invocationExpr, Func<ExpressionSyntax, T> waddleExpression);
        void OnIfStmt(IfStmtSyntax ifStmt, T expressionType);
        void OnReturnStmt(ReturnStmtSyntax returnStmt, T returnExprType);
        void OnAssignStmt(AssignStmtSyntax assignStmt, T waddleExpression);
        void OnDeclStmt(DeclStmtSyntax declStmt, T exprType);
    }
}