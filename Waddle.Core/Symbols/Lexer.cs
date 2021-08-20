using System;
using System.Collections.Generic;
using System.Text;

namespace Waddle.Core.Symbols
{
    /// <summary>
    /// The lexer turns the input source code into a stream of tokens. A token represents an atomic syntactical element,
    /// for example a string literal, an identifier, a number, the keyword `if` or the operator `+`.
    /// Usually a lexer is implemented as a state machine with one state per symbol class (word, number, operator,
    /// comment etc).
    /// Alternative implementations use regular expressions with a performance penalty.
    /// </summary>
    public class Lexer : IDisposable
    {
        private readonly CharReader _reader;
        private readonly StringBuilder _buffer = new();
        private char _currentChar;

        public Lexer(CharReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public IEnumerable<Token> Lex()
        {
            while (Read())
            {
                if (char.IsWhiteSpace(_currentChar))
                {
                    continue;
                }

                var token = _currentChar switch
                {
                    var ch when char.IsNumber(ch) => ReadNumber(),
                    var ch when CharIsBrace(ch) => ReadBrace(),
                    var ch when CharIsOperator(ch) => ReadOperator(),
                    var ch when CharIsTextCharacter(ch) => ReadText(),
                    var ch when CharIsSeparator(ch) => ReadSeparator(),
                    _ => throw new Exception(
                        $"unexpected input @{_reader.LineNumber}:{_reader.CharPosition} '{_currentChar}'"),
                };

                if (token != null)
                {
                    yield return token;
                }
            }
        }

        private Token ReadSeparator()
        {
            var (line, pos) = (_reader.LineNumber, _reader.CharPosition);

            return _currentChar switch
            {
                '.' => new Token(TokenType.Dot, _currentChar.ToString(), line, pos),
                ':' => new Token(TokenType.Colon, _currentChar.ToString(), line, pos),
                ';' => new Token(TokenType.Semicolon, _currentChar.ToString(), line, pos),
                ',' => new Token(TokenType.Comma, _currentChar.ToString(), line, pos),
                _ => new Token(TokenType.Unknown, _currentChar.ToString(), line, pos),
            };
        }

        private static bool CharIsSeparator(char ch)
        {
            return ".,;:".Contains(ch);
        }

        private Token? ReadOperator()
        {
            var (line, pos) = (_reader.LineNumber, _reader.CharPosition);
            var text = ReadLexemeWhile(CharIsOperator);
            if (text.StartsWith("//"))
            {
                // discard single-line comment
                ReadLexemeWhile(c => c != '\n');
                return null;
            }

            var tokenType = text switch
            {
                "+" => TokenType.Plus,
                "-" => TokenType.Minus,
                "*" => TokenType.Multiply,
                "/" => TokenType.Divide,
                "=" => TokenType.Equal,
                "==" => TokenType.Equals,
                "<" => TokenType.LessThan,
                ">" => TokenType.GreaterThan,
                "<=" => TokenType.LessEquals,
                ">=" => TokenType.GreaterEquals,
                "->" => TokenType.Arrow,
                "&" => TokenType.And,
                "|" => TokenType.Or,
                _ => TokenType.Unknown,
            };

            return new Token(tokenType, text, line, pos);
        }

        private static bool CharIsOperator(char ch)
        {
            return "+-*/|<>=&".Contains(ch);
        }

        private Token ReadText()
        {
            var (line, pos) = (_reader.LineNumber, _reader.CharPosition);
            var text = ReadLexemeWhile(CharIsAllowedIdentifierChar);
            var tokenType = text switch
            {
                "function" => TokenType.Function,
                "if" => TokenType.If,
                "var" => TokenType.Var,
                "print" => TokenType.Print,
                "return" => TokenType.Return,
                "for" => TokenType.For,
                "int" => TokenType.Int,
                "float" => TokenType.Float,
                "string" => TokenType.String,
                "bool" => TokenType.Bool,
                "char" => TokenType.Char,
                "buffer" => TokenType.Buffer,
                "regex" => TokenType.Regex,
                _ => TokenType.Identifier,
            };

            return new Token(tokenType, text, line, pos);
        }

        private static bool CharIsTextCharacter(char ch)
        {
            return char.IsLetter(ch) || "_$".Contains(ch);
        }

        private static bool CharIsAllowedIdentifierChar(char ch)
        {
            return char.IsNumber(ch) || CharIsTextCharacter(ch);
        }

        public void Dispose()
        {
            _reader.Dispose();
            GC.SuppressFinalize(this);
        }

        private static bool CharIsBrace(char ch)
        {
            return "(){}".Contains(ch);
        }

        private Token ReadBrace()
        {
            var (line, pos) = (_reader.LineNumber, _reader.CharPosition);
            var tokenType = _currentChar switch
            {
                '(' => TokenType.LParen,
                ')' => TokenType.RParen,
                '{' => TokenType.LBrace,
                '}' => TokenType.RBrace,
                _ => TokenType.Unknown,
            };
            return new Token(tokenType, _currentChar.ToString(), line, pos);
        }

        private bool Read()
        {
            var ch = _reader.Read();
            if (ch is < 0 or >= 65535)
            {
                return false;
            }

            _currentChar = (char)ch;
            return true;
        }

        private Token ReadNumber()
        {
            return ReadWhile(TokenType.Number, char.IsDigit);
        }

        private Token ReadWhile(TokenType tokenType, Predicate<char> predicate)
        {
            var (line, pos) = (_reader.LineNumber, _reader.CharPosition);
            return new Token(tokenType, ReadLexemeWhile(predicate), line, pos);
        }

        private string ReadLexemeWhile(Predicate<char> predicate)
        {
            var sb = _buffer.Clear();
            while (true)
            {
                if (predicate(_currentChar))
                {
                    sb.Append(_currentChar);
                }
                else
                {
                    _reader.Unread(_currentChar);
                    return sb.ToString();
                }

                if (Read() == false)
                {
                    return sb.ToString();
                }
            }
        }
    }
}
