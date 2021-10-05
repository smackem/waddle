using System;
using System.Collections.Generic;
using System.Xml.Xsl;
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
            Next();
            var expression = ParseExpression();
            return new DeclStmtSyntax(startToken, parameter, equalToken, expression);
        }

        private PrintStmtSyntax ParsePrintStatement()
        {
            var startToken = CurrentToken;
            var lParenToken = Expect(TokenType.LParen);
            Next();
            var argumentListSyntax = ParseExpressionList();
            
            Next();
            
            return new PrintStmtSyntax(startToken, lParenToken, argumentListSyntax);
        }

        private IList<ExpressionSyntax> ParseExpressionList()
        {
            var result = new List<ExpressionSyntax>();
            
            if (CurrentToken.Type == TokenType.RParen)
            {
                return result;
            }
            
            result.Add(ParseExpression());
            while (CurrentToken.Type == TokenType.Comma)
            {
                Next();
                result.Add(ParseExpression());
            }

            return result;
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
            Next();
            var invocationExpression = ParseInvocationExpression();
            return new InvocationStmtSyntax(startToken, invocationExpression);
        }

        private ReturnStmtSyntax ParseReturnStatement()
        {
            var startToken = CurrentToken;
            Next();
            var expression = ParseExpression();
            return new ReturnStmtSyntax(startToken, expression);
        }

        private IfStmtSyntax ParseIfStatement()
        {
            var startToken = CurrentToken;
            Next();
            var expression = ParseExpression();
            var block = ParseBlock();
            return new IfStmtSyntax(startToken, expression, block);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseLogicalExpression();
        }

        private ExpressionSyntax ParseLogicalExpression()
        {
            var startToken = CurrentToken;
            ExpressionSyntax left = ParseRelationalExpression();
            while (true)
            {
                switch (CurrentToken.Type)
                {
                    case TokenType.Or:
                        Next();
                        left = new LogicalExpressionSyntax(startToken, left, ParseRelationalExpression(),
                            LogicalOperator.Or);
                        break;
                    case TokenType.And:
                        Next();
                        left = new LogicalExpressionSyntax(startToken, left, ParseRelationalExpression(),
                            LogicalOperator.And);
                        break;
                    default:
                        return left;
                }
            }
        }

        private ExpressionSyntax ParseRelationalExpression()
        {
            var startToken = CurrentToken;
            ExpressionSyntax left = ParseTermExpression();
            while (true)
            {
                switch (CurrentToken.Type)
                {
                    case TokenType.Equals:
                        Next();
                        left = new RelationalExpressionSyntax(startToken, left, ParseTermExpression(), RelationalOperator.Eq);
                        break;
                    case TokenType.NotEquals:
                        Next();
                        left = new RelationalExpressionSyntax(startToken, left, ParseTermExpression(), RelationalOperator.Ne);
                        break;
                    case TokenType.LessEquals:
                        Next();
                        left = new RelationalExpressionSyntax(startToken, left, ParseTermExpression(), RelationalOperator.Le);
                        break;
                    case TokenType.LessThan:
                        Next();
                        left = new RelationalExpressionSyntax(startToken, left, ParseTermExpression(), RelationalOperator.Lt);
                        break;
                    case TokenType.GreaterEquals:
                        Next();
                        left = new RelationalExpressionSyntax(startToken, left, ParseTermExpression(), RelationalOperator.Ge);
                        break;
                    case TokenType.GreaterThan:
                        Next();
                        left = new RelationalExpressionSyntax(startToken, left, ParseTermExpression(), RelationalOperator.Gt);
                        break;
                    default:
                        return left;
                }
            }
        }

        private ExpressionSyntax ParseTermExpression()
        {
            var startToken = CurrentToken;
            ExpressionSyntax left = ParseProductExpression();
            while (true)
            {
                switch (CurrentToken.Type)
                {
                    case TokenType.Plus:
                        Next();
                        left = new TermExpressionSyntax(startToken, left, ParseProductExpression(), TermOperator.Plus);
                        break;
                    case TokenType.Minus:
                        Next();
                        left = new TermExpressionSyntax(startToken, left, ParseProductExpression(), TermOperator.Minus);
                        break;
                    default:
                        return left;
                }
            }
        }

        

        private ExpressionSyntax ParseProductExpression()
        {
            var startToken = CurrentToken;
            ExpressionSyntax left = ParseAtom();
            while (true)
            {
                switch (CurrentToken.Type)
                {
                    case TokenType.Multiply:
                        Next();
                        left = new ProductExpressionSyntax(startToken, left, ParseAtom(), ProductOperator.Times);
                        break;
                    case TokenType.Divide:
                        Next();
                        left = new ProductExpressionSyntax(startToken, left, ParseAtom(), ProductOperator.Divide);
                        break;
                    default:
                        return left;
                }
            }
        }

        private AtomSyntax ParseAtom()
        {
            if (CurrentToken.Type == TokenType.Arrow)
            {
                return ParseInvocationExpression();
            }
            if (CurrentToken.Type == TokenType.Number)
            {
                return new IntegerLiteralAtom(CurrentToken, int.Parse(CurrentToken.Lexeme));
            }
            if (CurrentToken.Type == TokenType.Identifier)
            {
                return new IdentifierAtom(CurrentToken, CurrentToken.Lexeme);
            }
            throw new Exception($"syntax error @({CurrentToken.LineNumber}:{CurrentToken.CharPosition}) - atom expected - found {CurrentToken.Type}");   
        }

        private InvocationExpressionSyntax ParseInvocationExpression()
        {
            var identToken = Expect(TokenType.Identifier);
            var lParenToken = Expect(TokenType.LParen);
            Next();
            var exprList = ParseExpressionList();
            var rParenToken = CurrentToken;
            Next();
            return new InvocationExpressionSyntax(identToken, lParenToken, exprList, rParenToken);
        }

        private IEnumerable<ParameterDecSyntax> ParseParameterList()
        {
            var result = new List<ParameterDecSyntax>();
            
            if (CurrentToken.Type == TokenType.RParen)
            {
                return result;
            }
            
            result.Add(ParseParameter());
            while (CurrentToken.Type == TokenType.Comma)
            {
                Next();
                result.Add(ParseParameter());
            }
            
            return result;
        }

        private ParameterDecSyntax ParseParameter()
        {
            var startToken = Assert(TokenType.Identifier);
            var colonToken = Expect(TokenType.Colon);
            var typeToken = CurrentToken;
            ExpectTypeToken();
            Next();
            return new ParameterDecSyntax(startToken, startToken.Lexeme, colonToken, new TypeSyntax(colonToken, typeToken));
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