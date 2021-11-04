using System;
using System.Linq;
#if WADDLER
namespace Waddle.Core.Syntax.Ast
{
    public class Waddler<T>
    {
        private readonly IWaddleListener<T> _listener;

        public Waddler(IWaddleListener<T> listener)
        {
            _listener = listener;
        }
        
        public void WaddleProgram(ProgramSyntax program)
        {
            _listener.OnProgram(program);

            foreach (var function in program.FunctionDeclarations)
            {
                WaddleFunctionDeclaration(function);
            }
        }

        private void WaddleFunctionDeclaration(FunctionDeclSyntax function)
        {
            _listener.OnFunctionDeclaration(function);

            var stmts = function.Body.Statements.ToArray();

            foreach (var stmt in stmts)
            {
                WaddleStmt(stmt);
            }
        }

        private void WaddleStmt(StatementSyntax stmt)
        {
            switch (stmt)
            {
                case DeclStmtSyntax declStmt:
                    WaddleDeclStmt(declStmt);
                    break;
                case AssignStmtSyntax assignStmt:
                    WaddleAssignStmt(assignStmt);
                    break;
                case ReturnStmtSyntax returnStmt:
                    WaddleReturnStmt(returnStmt);
                    break;
                case IfStmtSyntax ifStmt:
                    WaddleIfStmt(ifStmt);
                    break;
                case InvocationStmtSyntax invocationStmt:
                    // check inner InvocationExpressionSyntax
                    WaddleExpression(invocationStmt.Expression);
                    break;
                case PrintStmtSyntax printStmt:
                    // check that expr is not void
                    WaddlePrintStmt(printStmt);
                    break;
            }
        }

        private void WaddlePrintStmt(PrintStmtSyntax printStmt)
        {
            _listener.OnPrintStmt(printStmt);

            foreach (var printStmtArgument in printStmt.Arguments)
            {
                WaddleExpression(printStmtArgument);
            }
        }

        private void WaddleIfStmt(IfStmtSyntax ifStmt)
        {
            _listener.OnIfStmt(ifStmt);

            // check that expr is boolean
            WaddleExpression(ifStmt.Expression);
        }

        private void WaddleReturnStmt(ReturnStmtSyntax returnStmt)
        {
            _listener.OnReturnStmt(ifStmt);

            WaddleExpression(returnStmt.Expression);
        }

        private void WaddleAssignStmt(AssignStmtSyntax assignStmt)
        {
            _listener.OnAssignStmt(ifStmt);

            WaddleExpression(assignStmt.Expression);
        }

        private void WaddleDeclStmt(DeclStmtSyntax declStmt)
        {
            _listener.OnDeclStmt(ifStmt);

            WaddleExpression(declStmt.Expression);
        }

        private T WaddleExpression(ExpressionSyntax exprStmt)
        {
            return exprStmt switch
            {
                LogicalExpressionSyntax logicalExpr => WaddleLogicalExpr(logicalExpr),
                ProductExpressionSyntax productExpr => WaddleProductExpr(productExpr),
                RelationalExpressionSyntax relationalExpr => WaddleRelationalExpr(relationalExpr),
                TermExpressionSyntax termExpr => WaddleTermExpr(termExpr),
                InvocationExpressionSyntax invocationExpr => WaddleInvocationExpr(invocationExpr),
                BoolLiteralAtom atom => WaddleBoolLiteral(atom),
                IntegerLiteralAtom atom => WaddleIntegerLiteral(atom),
                StringLiteralAtom atom => WaddleStringLiteral(atom),
                IdentifierAtom atom => WaddleIdentifier(atom),
                _ => throw new ArgumentOutOfRangeException(nameof(exprStmt)),
            };
        }

        private T WaddleTermExpr(TermExpressionSyntax termExpr)
        {
            // _listener.OnTermExpr(termExpr);
            // WaddleExpression(termExpr.Left);
            // WaddleExpression(termExpr.Right);
            //return _listener.OnTermExpr(WaddleExpression(termExpr.Left), WaddleExpression(termExpr.Right));
        }

        private TypeSymbol WaddleProductExpr(ProductExpressionSyntax productExpr)
        {
            if (WaddleExpression(productExpr.Left) != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Number @({productExpr.Left.StartToken.LineNumber}{productExpr.Left.StartToken.CharPosition}).");
            }
            if (WaddleExpression(productExpr.Right) != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Number @({productExpr.Right.StartToken.LineNumber}{productExpr.Right.StartToken.CharPosition}).");
            }
            return TypeSymbol.Integer;
        }

        private TypeSymbol WaddleLogicalExpr(LogicalExpressionSyntax logicalExpr)
        {
            if (WaddleExpression(logicalExpr.Left) != TypeSymbol.Bool)
            {
                throw new SemanticErrorException($"Not an Bool @({logicalExpr.Left.StartToken.LineNumber}{logicalExpr.Left.StartToken.CharPosition}).");
            }
            if (WaddleExpression(logicalExpr.Right) != TypeSymbol.Bool)
            {
                throw new SemanticErrorException($"Not an Bool @({logicalExpr.Right.StartToken.LineNumber}{logicalExpr.Right.StartToken.CharPosition}).");
            }
            return TypeSymbol.Bool;
        }

        private TypeSymbol WaddleRelationalExpr(RelationalExpressionSyntax relationalExpr)
        {
            // check relationExpr.Left and relationalExpr.Right
            if (WaddleExpression(relationalExpr.Left) != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Integer @({relationalExpr.Left.StartToken.LineNumber}{relationalExpr.Left.StartToken.CharPosition}).");
            }
            if (WaddleExpression(relationalExpr.Right) != TypeSymbol.Integer)
            {
                throw new SemanticErrorException($"Not an Integer @({relationalExpr.Right.StartToken.LineNumber}{relationalExpr.Right.StartToken.CharPosition}).");
            }
            
            return TypeSymbol.Bool;
        }

        private TypeSymbol WaddleInvocationExpr(InvocationExpressionSyntax invocationExpr)
        {
            if (_functions.ContainsKey(invocationExpr.Identifier) == false)
            {
                throw new SemanticErrorException($"function {invocationExpr.Identifier} is not defined");
            }

            FunctionDecl functionDecl = _functions[invocationExpr.Identifier]!;
            var parameters = functionDecl.Parameters;
            var arguments = invocationExpr.Arguments.ToArray();

            if (parameters.Count != arguments.Length)
            {
                throw new SemanticErrorException($"function {invocationExpr.Identifier} has {functionDecl.Parameters.Count} parameters, {invocationExpr.Arguments.Count()} arguments where given.");
            }

            // check that for all parameters: IsAssignableFrom(param.Type, arg.Type)
            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                var exprType = WaddleExpression(arguments[i]);
                if (IsAssignableFrom(parameter.Type!, exprType) == false)
                {
                    throw new SemanticErrorException($"Parameter {parameter} requires type {parameter.Type}, but type {exprType} was given.");
                }
            }
            
            return _functions[invocationExpr.Identifier].Type ?? TypeSymbol.Void;
        }
    }
}
#endif