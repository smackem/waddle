using System;
using System.Collections;
using System.Collections.Generic;
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
                    // uncomment when all rules are done:
                    // default:
                    //     throw new LexingException($"unexpected input @{_reader.LineNumber}:{_reader.CharPosition} '{_currentChar}'");
                }
            }
        }

        public void Dispose() {
            _reader?.Dispose();
        }

        private bool Read() {
            var ch = _reader.Read();
            if (ch < 0) {
                return false;
            }
            _currentChar = (char) ch;
            return true;
        }

        private Token ReadNumber() {
            return ReadWhile(TokenType.Number, char.IsDigit);
        }

        Token ReadWhile(TokenType tokenType, Predicate<char> predicate) {
            var sb = _buffer.Clear();
            var (line, pos) = (_reader.LineNumber, _reader.CharPosition);

            while (true) {
                if (predicate(_currentChar)) {
                    sb.Append(_currentChar);
                } else {
                    _reader.Unread(_currentChar);
                    return new Token(tokenType, sb.ToString(), line, pos);
                }

                if (Read() == false) {
                    return new Token(tokenType, sb.ToString(), line, pos);
                }
            }
        }
    }
}