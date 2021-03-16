using System.Collections;
using System.Collections.Generic;

namespace Waddle.Core.Symbols {
    /// <summary>
    /// The lexer turns the input source code into a stream of tokens. A token represents an atomic syntactical element,
    /// for example a string literal, an identifier, a number, the keyword `if` or the operator `+`.
    /// Usually a lexer is implemented as a state machine with one state per symbol class (word, number, operator,
    /// comment etc).
    /// Alternative implementations use regular expressions with a performance penalty.
    /// </summary>
    public class Lexer {
        public IEnumerable<Token> Lex(CharReader input) {
            int ch;
            while ((ch = input.Read()) > 0) {
                switch (ch) {
                    case '+'
                        yield return new Token(TokenType.Plus, ch, input.CharPosition, input.LineNumber)
                }
            }
            yield return new Token();
        }
    }
}
