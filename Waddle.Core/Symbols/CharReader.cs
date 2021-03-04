using System;
using System.IO;

namespace Waddle.Core.Symbols {
    public class CharReader : IDisposable {
        private readonly TextReader _reader;
        
        public CharReader(TextReader reader) {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public int LineNumber { get; private set; }
        public int CharPosition { get; private set; }

        public int Read() {
            var ch = (char) _reader.Read();
            switch (ch) {
                case '\n':
                    LineNumber++;
                    CharPosition = 0;
                    break;
            }

            CharPosition++;
            return ch;
        }

        public void Dispose() {
            _reader.Dispose();
        }
    }
}