using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waddle.Core.Symbols {
    /// <summary>
    /// The lexer turns the input source code into a stream of tokens. A token represents an atomic syntactical element,
    /// for example a string literal, an identifier, a number, the keyword `if` or the operator `+`.
    /// Usually a lexer is implemented as a state machine with one state per symbol class (word, number, operator,
    /// comment etc).
    /// Alternative implementations use regular expressions with a performance penalty.
    /// </summary>
    public class Lexer : IDisposable {
        private readonly CharReader _reader;
        private readonly StringBuilder _buffer = new StringBuilder();
        private char _currentChar;

        public Lexer(CharReader reader) {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public IEnumerable<Token> Lex() {
            while (Read()) {
                switch (_currentChar) {
                    case { } ch when char.IsNumber(ch):
                        yield return ReadNumber();
                        break;
                    case { } ch when char.IsWhiteSpace(ch):
                        break;
                    case { } ch when CharIsBrace(ch):
                        yield return ReadBrace();
                        break;
                    case { } ch when CharIsOperator(ch):
                        yield return ReadOperator();
                        break;
                    case { } ch when CharIsTextCharacter(ch):
                        yield return ReadText();
                        break;
                    case{ } ch when CharIsSeparator(ch):
                        yield return ReadSeparator();
                        break;
                    // uncomment when all rules are done:
                    // default:
                    //     throw new LexingException($"unexpected input @{_reader.LineNumber}:{_reader.CharPosition} '{_currentChar}'");
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

        private readonly char[] _separators = {'.', ',', ';', ':'}; 
        private bool CharIsSeparator(char ch)
        {
            return _separators.Contains(ch);
        }

        private Token ReadOperator()
        {
            var (line, pos) = (_reader.LineNumber, _reader.CharPosition);
            string text = ReadContentUntilPredicate(CharIsOperator);
            if (text == "//")
            {
                //Comment has been found, read until next line
                _reader.Unread('/');
                _reader.Unread('/');
                return ReadWhile(TokenType.Comment, c => c != '\n');
            }
            TokenType tokenType = text switch
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
                _ => TokenType.Unknown
            };
            return new Token(tokenType, text, line, pos);
        }

        private readonly char[] _operators = {'+', '-', '*', '/', '|', '<', '>', '=', '&'}; 
        private bool CharIsOperator(char ch)
        {
            return _operators.Contains(ch);
        }

        private Token ReadText()
        {
            var (line, pos) = (_reader.LineNumber, _reader.CharPosition);
            string text = ReadContentUntilPredicate(CharIsAllowedIdentifierChar);
            TokenType tokenType = text.ToLower() switch
            {
                "function" => TokenType.Function,
                "if" => TokenType.If,
                "var" => TokenType.Var,
                "print" => TokenType.Print,
                "return" => TokenType.Return,
                { } type when TextIsTypeDefinition(type) => TokenType.Type,
                _ => TokenType.Identifier
            };

            return new Token(tokenType, text, line, pos);
        }

        private bool TextIsTypeDefinition(string type)
        {
            switch (type)
            {
                case "int":
                case "float":
                case "string":
                case "bool":
                case "char":
                case "buffer":
                case "regex":
                    return true;
            }

            return false;
        }


        readonly char[] _additionalAllowedCharacters = {'_', '$'};
        private bool CharIsTextCharacter(char ch)
        {
            return char.IsLetter(ch) || _additionalAllowedCharacters.Contains(ch);
        }

        private bool CharIsAllowedIdentifierChar(Char ch)
        {
            return char.IsNumber(ch) || CharIsTextCharacter(ch);
        }

        public void Dispose() {
            _reader?.Dispose();
        }
        
        readonly char[] _braces = {'(', ')', '{', '}'};
        private bool CharIsBrace(char ch)
        {
            return _braces.Contains(ch);
        }

        private Token ReadBrace()
        {
            var (line, pos) = (_reader.LineNumber, _reader.CharPosition);
            var tokenType = _currentChar switch
            {
                '(' => TokenType.RParen,
                ')' => TokenType.LParen,
                '{' => TokenType.RBrace,
                '}' => TokenType.LBrace,
                _ => TokenType.Unknown
            };
            return new Token(tokenType, _currentChar.ToString(), line, pos);
        }

        private bool Read() {
            var ch = _reader.Read();
            if (ch < 0 || ch >= 65535) {
                return false;
            }
            _currentChar = (char) ch;
            return true;
        }

        private Token ReadNumber() {
            return ReadWhile(TokenType.Number, char.IsDigit);
        }

        Token ReadWhile(TokenType tokenType, Predicate<char> predicate) {
            var (line, pos) = (_reader.LineNumber, _reader.CharPosition);
            return new Token(tokenType, ReadContentUntilPredicate(predicate), line, pos);
        }

        string ReadContentUntilPredicate(Predicate<char> predicate)
        {
            var sb = _buffer.Clear();
            while (true) {
                if (predicate(_currentChar)) {
                    sb.Append(_currentChar);
                } else {
                    _reader.Unread(_currentChar);
                    return sb.ToString();
                }

                if (Read() == false) {
                    return sb.ToString();
                }
            }
        }
    }
}