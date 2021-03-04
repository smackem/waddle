namespace Waddle.Core.Symbols {
    /// <summary>
    /// The lexer turns the input source code into a stream of tokens. A token represents an atomic syntactical element,
    /// for example a string literal, an identifier, a number, the keyword `if` or the operator `+`.
    /// Usually a lexer is implemented as a state machine with one state per symbol class (word, number, operator,
    /// comment etc).
    /// Alternative implementations use regular expressions with a performance penalty.
    /// </summary>
    public class Lexer {
    }
}