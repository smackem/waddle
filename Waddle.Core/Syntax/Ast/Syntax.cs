using System.Collections.Generic;
using Waddle.Core.Symbols;

namespace Waddle.Core.Syntax.Ast
{
    public abstract record Syntax(Token StartToken);

    public record ProgramSyntax
        (Token StartToken, IEnumerable<FunctionDeclSyntax> FunctionDeclarations) : Syntax(StartToken);

    public record FunctionDeclSyntax(Token StartToken, string Name, ParameterListSyntax? Parameters, TypeSyntax? ReturnType, BlockSyntax Body);

    public record TypeSyntax(Token StartToken, Token TypeToken) : Syntax(StartToken);

    public record ParameterListSyntax
        (Token StartToken, IEnumerable<ParameterDecSyntax> Parameters) : Syntax(StartToken);

    public record ParameterDecSyntax(Token StartToken, Token ColonToken, TypeSyntax TypeSyntax) : Syntax(StartToken);

    public record BlockSyntax(Token StartToken, IEnumerable<StatementSyntax> Statements) : Syntax(StartToken);

    public abstract record StatementSyntax(Token StartToken) : Syntax(StartToken);

    public record ReturnStmtSyntax(Token StartToken, ExpressionSyntax Expression) : StatementSyntax(StartToken);

    public record IfStmtSyntax (Token StartToken, ExpressionSyntax Expression, BlockSyntax Body) : StatementSyntax(StartToken);

    public record AssignStmtSyntax (Token StartToken, Token EqualToken, ExpressionSyntax Expression) : StatementSyntax(StartToken);

    public record PrintStmtSyntax (Token StartToken, Token LParenToken, ArgumentListSyntax? Arguments) : StatementSyntax(StartToken);

    public record DeclStmtSyntax(Token StartToken, ParameterDecSyntax ParameterDecSyntax, Token EqualToken,  ExpressionSyntax Expression) : StatementSyntax(StartToken);

    public record InvocationStmtSyntax(Token StartToken, InvocationExpressionSyntax Expression) : StatementSyntax(StartToken);

    public class ExpressionBase
    {
        public Token StartToken { get; set; }

        public ExpressionBase(Token startToken)
        {
            this.StartToken = startToken;
        }
    }
    
    public class ExpressionSyntax : ExpressionBase
    {
        public ExpressionBase FirstExpression { get; set;}
        public IEnumerable<ExpressionOperator> InnerExpressions { get; set;}

        public ExpressionSyntax(Token startToken, ExpressionBase firstExpression,
            IEnumerable<ExpressionOperator> innerExpressions): base(startToken)
        {
            this.FirstExpression = firstExpression;
            this.InnerExpressions = innerExpressions;
        }
    }

    public record ExpressionOperator(Token OperatorToken, ExpressionBase Expression);

    public class LogicalOrExpressionSyntax : ExpressionSyntax
    {
        public LogicalOrExpressionSyntax(Token startToken, LogicalAndExpressionSyntax firstExpression, IEnumerable<ExpressionOperator> innerExpressions) : base(startToken, firstExpression, innerExpressions)
        {
            
        }
    };
    
    public class LogicalAndExpressionSyntax : ExpressionSyntax
    {
        public LogicalAndExpressionSyntax(Token startToken, RelationalExpressionSyntax firstExpression, IEnumerable<ExpressionOperator> innerExpressions) : base(startToken, firstExpression, innerExpressions)
        {
            
        }
    };
    
    public class RelationalExpressionSyntax : ExpressionSyntax
    {
        public RelationalExpressionSyntax(Token startToken, TermExpressionSyntax firstExpression, IEnumerable<ExpressionOperator> innerExpressions) : base(startToken, firstExpression, innerExpressions)
        {
            
        }
    };

    public class TermExpressionSyntax : ExpressionSyntax
    {
        public TermExpressionSyntax(Token startToken, ProductExpressionSyntax firstExpression, IEnumerable<ExpressionOperator> innerExpressions) : base(startToken, firstExpression, innerExpressions)
        {
            
        }
    };
    
    public class ProductExpressionSyntax : ExpressionSyntax
    {
        public ProductExpressionSyntax(Token startToken, AtomSyntax firstExpression, IEnumerable<ExpressionOperator> innerExpressions) : base(startToken, firstExpression, innerExpressions)
        {
            
        }
    };

    public class AtomSyntax: ExpressionBase
    {
        public AtomSyntax(Token startToken) : base(startToken){}
    }

    public class InvocationExpressionSyntax : AtomSyntax
    {
        public Token Identifier { get; }
        public Token LParen { get; }
        public ArgumentListSyntax? Arguments { get; }
        public Token RParen { get; }

        public InvocationExpressionSyntax(Token startToken, Token identifier, Token lParen, ArgumentListSyntax? arguments, Token rParen) : base(startToken)
        {
            this.Identifier = identifier;
            this.LParen = lParen;
            this.Arguments = arguments;
            this.RParen = rParen;
        }
    }
    
    public record ArgumentListSyntax(Token StartToken, ExpressionSyntax? Expression, IEnumerable<AdditionalArgument>? AdditionalArguments) : Syntax(StartToken);

    public record AdditionalArgument(Token CommaToken, ExpressionSyntax Expression);
}