using System;
using System.Collections.Generic;
using Waddle.Core.Symbols;
using Waddle.Core.Syntax.Ast;

namespace Waddle.Core.Syntax
{
    public class Parser
    {
        private readonly IEnumerator<Token> _enumerator;

        public Parser(IEnumerable<Token> tokens)
        {
            _enumerator = tokens.GetEnumerator();
        }

        public Ast.Syntax Parse()
        {
            Next();
            return ParseProgram();
        }

        private Ast.Syntax ParseProgram()
        {
            var functionDecls = new List<FunctionDeclSyntax>();
            var startToken = CurrentToken;
            while (CurrentToken.Type != TokenType.Eof)
            {
                functionDecls.Add(ParseFunctionDecl());
            }

            return new ProgramSyntax(startToken, functionDecls);
        }

        private FunctionDeclSyntax ParseFunctionDecl()
        {
            var startToken = Assert(TokenType.Function);
            var identifierToken = Expect(TokenType.Identifier);
            
            Expect(TokenType.LParen);
            Next();
            
            var parameters = ParseParameterList();
            Assert(TokenType.RParen);

            Next();
            TypeSyntax? typeSyntax = null;
            if (CurrentToken.Type == TokenType.Arrow)
            {
                typeSyntax = ExpectTypeToken();
            }

            var blockSyntax = ParseBlock();
            return new FunctionDeclSyntax(startToken, identifierToken.Lexeme, parameters, typeSyntax, blockSyntax);
            // CurrentToken.Lexeme
        }

        private BlockSyntax ParseBlock()
        {
            if (CurrentToken.Type != TokenType.LBrace)
            {
                Expect(TokenType.LBrace);
            }

            var startToken = CurrentToken;
            
            Next();
            var statements = new List<StatementSyntax>();
            while (CurrentToken.Type != TokenType.RBrace)
            {
                statements.Add(ParseStatement());
            }
            Next();

            return new BlockSyntax(startToken, statements);
        }

        private StatementSyntax ParseStatement()
        {
            var token = CurrentToken;
            StatementSyntax result;
            switch (token.Type)
            {
                case TokenType.Return:
                    result = ParseReturnStatement();
                    break;
                case TokenType.Identifier:
                    result = ParseAssignStatement();
                    break;
                case TokenType.Print:
                    result = ParsePrintStatement();
                    break;
                case TokenType.Var:
                    result = ParseDeclStatement();
                    break;
                case TokenType.Arrow:
                    result = ParseInvocationStatement();
                    break;
                case TokenType.If:
                    return ParseIfStatement();
                default:
                    throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - Statement expected - found {token.Type}");
            }
            Assert(TokenType.Semicolon);
            Next();
            return result;
        }

        private DeclStmtSyntax ParseDeclStatement()
        {
            var startToken = CurrentToken;
            Next();
            var parameter = ParseParameter();
            var equalToken = Assert(TokenType.Equal);
            var expression = ParseExpression();
            return new DeclStmtSyntax(startToken, parameter, equalToken, expression);
        }

        private PrintStmtSyntax ParsePrintStatement()
        {
            var startToken = CurrentToken;
            var lParenToken = Expect(TokenType.LParen);

            var argumentListSyntax = ParseExpressionList();
            
            Next();
            
            return new PrintStmtSyntax(startToken, lParenToken, argumentListSyntax);
        }

        private ArgumentListSyntax? ParseExpressionList()
        {
            var startToken = CurrentToken;
            if (CurrentToken.Type == TokenType.RParen)
            {
                return null;
            }
            var firstExpression = ParseExpression();
            var addArgs = new List<AdditionalArgument>();
            while (CurrentToken.Type != TokenType.RParen)
            {
                var addStartToken = CurrentToken;
                var additionExpression = ParseExpression();
                addArgs.Add(new AdditionalArgument(addStartToken, additionExpression));
            }

            return new ArgumentListSyntax(startToken, firstExpression, addArgs);
        }

        private AssignStmtSyntax ParseAssignStatement()
        {
            var startToken = CurrentToken;
            var equalToken = Expect(TokenType.Equal);
            var expression = ParseExpression();
            return new AssignStmtSyntax(startToken, equalToken, expression);
        }

        private InvocationStmtSyntax ParseInvocationStatement()
        {
            var startToken = CurrentToken;
            var invocationExpression = ParseInvocationExpression();
            return new InvocationStmtSyntax(startToken, invocationExpression);
        }

        private ReturnStmtSyntax ParseReturnStatement()
        {
            var startToken = CurrentToken;
            var expression = ParseExpression();
            return new ReturnStmtSyntax(startToken, expression);
        }

        private IfStmtSyntax ParseIfStatement()
        {
            var startToken = CurrentToken;
            var expression = ParseExpression();
            var block = ParseBlock();
            return new IfStmtSyntax(startToken, expression, block);
        }

        private ExpressionSyntax ParseExpression()
        {
            var startToken = CurrentToken;
            return new ExpressionSyntax(startToken, ParseLogicalOrExpression(), new List<ExpressionOperator>());
        }

        private LogicalOrExpressionSyntax ParseLogicalOrExpression()
        {
            var startToken = CurrentToken;
            var firstLogicalAndExpression = ParseLogicalAndExpression();
            var additionalExpressions = new List<ExpressionOperator>();
            while (true)
            {
                if (CurrentToken.Type != TokenType.Or)
                {
                    return new LogicalOrExpressionSyntax(startToken, firstLogicalAndExpression, additionalExpressions);
                }
                var operatorToken = CurrentToken;
                LogicalAndExpressionSyntax additionalExpression = ParseLogicalAndExpression();
                additionalExpressions.Add(new ExpressionOperator(operatorToken, additionalExpression));
            }
        }

        private LogicalAndExpressionSyntax ParseLogicalAndExpression()
        {
            var startToken = CurrentToken;
            var firstRelationExpression = ParseRelationalExpression();
            var additionalExpressions = new List<ExpressionOperator>();
            while (true)
            {
                if (CurrentToken.Type != TokenType.And)
                {
                    return new LogicalAndExpressionSyntax(startToken, firstRelationExpression, additionalExpressions);
                }

                var operatorToken = CurrentToken;
                var additionalExpression = ParseRelationalExpression();
                additionalExpressions.Add(new ExpressionOperator(operatorToken, additionalExpression));
            }
        }

        private RelationalExpressionSyntax ParseRelationalExpression()
        {
            var startToken = CurrentToken;
            var firstTermExpression = ParseTermExpression();
            if (IsRelationalOperator(CurrentToken) == false)
            {
                return new RelationalExpressionSyntax(startToken, firstTermExpression, new List<ExpressionOperator>());
            }

            var operatorToken = CurrentToken;
            var additionalTermExpression = ParseTermExpression();
            return new RelationalExpressionSyntax(startToken, firstTermExpression, new List<ExpressionOperator>{new ExpressionOperator(operatorToken, additionalTermExpression)});
        }

        private static bool IsRelationalOperator(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Equals:
                case TokenType.NotEquals:
                case TokenType.LessEquals:
                case TokenType.LessThan:
                case TokenType.GreaterEquals:
                case TokenType.GreaterThan:
                    return true;
            }

            return false;
        }

        private TermExpressionSyntax ParseTermExpression()
        {
            var startToken = CurrentToken;
            var firstProductExpression = ParseProductExpression();
            var additionalExpressions = new List<ExpressionOperator>();
            while (true)
            {
                if (IsTermOperator(CurrentToken) == false)
                {
                    return new TermExpressionSyntax(startToken, firstProductExpression, additionalExpressions);
                }
                
                var operatorToken = CurrentToken;
                var additionalProductExpression = ParseProductExpression();
                additionalExpressions.Add(new ExpressionOperator(operatorToken, additionalProductExpression));
            }
        }

        private static bool IsTermOperator(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                    return true;
            }

            return false;
        }

        private ProductExpressionSyntax ParseProductExpression()
        {
            var startToken = CurrentToken;
            Next();
            var firstAtom = ParseAtom();
            var additionalAtoms = new List<ExpressionOperator>();
            while (true)
            {
                if (IsProductOperator(CurrentToken) == false)
                {
                    return new ProductExpressionSyntax(startToken, firstAtom, additionalAtoms);
                }
                
                var operatorToken = CurrentToken;
                var additionalAtom = ParseAtom();
                additionalAtoms.Add(new ExpressionOperator(operatorToken, additionalAtom));
            }
        }

        private static bool IsProductOperator(Token currentToken)
        {
            return currentToken.Type is TokenType.Multiply or TokenType.Divide;
        }

        private AtomSyntax ParseAtom()
        {
            if (CurrentToken.Type == TokenType.Arrow)
            {
                return ParseInvocationExpression();
            }
            else if (CurrentToken.Type == TokenType.Number)
            {
            }
            else if (CurrentToken.Type == TokenType.Identifier)
            {
            }
            else
            {
                throw new Exception($"syntax error @({CurrentToken.LineNumber}:{CurrentToken.CharPosition} - atom expected - found {CurrentToken.Type}");   
            }
            
            Token atomToken = CurrentToken;
            Next();
            return new AtomSyntax(atomToken);
        }

        private InvocationExpressionSyntax ParseInvocationExpression()
        {
            var startToken = CurrentToken;
            var identToken = Expect(TokenType.Identifier);
            var lParenToken = Expect(TokenType.LParen);
            var exprList = ParseExpressionList();
            var rParenToken = CurrentToken;
            Next();
            return new InvocationExpressionSyntax(startToken, identToken, lParenToken, exprList, rParenToken);
        }

        private ParameterListSyntax ParseParameterList()
        {
            var startToken = CurrentToken;
            var result = new List<ParameterDecSyntax>();
            
            if (CurrentToken.Type == TokenType.RParen)
            {
                return new ParameterListSyntax(startToken, result);
            }
            
            result.Add(ParseParameter());
            while (CurrentToken.Type == TokenType.Comma)
            {
                Next();
                result.Add(ParseParameter());
            }
            
            return new ParameterListSyntax(startToken, result);
        }

        private ParameterDecSyntax ParseParameter()
        {
            var startToken = Assert(TokenType.Identifier);
            var colonToken = Expect(TokenType.Colon);
            var typeToken = CurrentToken;
            ExpectTypeToken();
            Next();
            return new ParameterDecSyntax(startToken, colonToken, new TypeSyntax(colonToken, typeToken));
        }

        private TypeSyntax ExpectTypeToken()
        {
            var startToken = CurrentToken;
            var token = Next();
            switch (token.Type)
            {
                case TokenType.Bool:
                case TokenType.String:
                case TokenType.Float:
                case TokenType.Int:
                case TokenType.Char:
                    return new TypeSyntax(startToken, token);
            }
            throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - Type expected - found {token.Type}");
        }
        
        private Token Expect(TokenType tokenType)
        {
            var token = Next();
            if (token.Type != tokenType)
            {
                throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - expected {tokenType} - found {token.Type}");
            }

            return token;
        }

        private Token Assert(TokenType tokenType)
        {
            var token = CurrentToken;
            if (token.Type != tokenType)
            {
                throw new Exception($"syntax error @({token.LineNumber}:{token.CharPosition} - expected {tokenType} - found {token.Type}");
            }

            return token;
        }

        private Token CurrentToken => _enumerator.Current ?? new Token(TokenType.Eof, string.Empty, 0, 0);

        private Token Next()
        {
            return _enumerator.MoveNext()
                ? _enumerator.Current
                : new Token(TokenType.Eof, string.Empty, 0, 0);
        }
    }
}