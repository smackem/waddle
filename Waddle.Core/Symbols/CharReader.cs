using System;
using System.IO;

namespace Waddle.Core.Symbols
{
    public class CharReader : IDisposable
    {
        private readonly TextReader _reader;
        private char? _head;

        public CharReader(TextReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public int LineNumber { get; private set; } = 1;
        public int CharPosition { get; private set; }

        public int Read()
        {
            if (_head != null)
            {
                var head = _head.Value;
                _head = null;
                return head;
            }

            var ch = (char) _reader.Read();
            if (ch == '\n')
            {
                LineNumber++;
                CharPosition = 0;
            }

            CharPosition++;
            return ch;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public void Unread(char ch)
        {
            _head = ch;
        }
    }
}