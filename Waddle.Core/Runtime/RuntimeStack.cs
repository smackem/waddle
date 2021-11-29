using System;

namespace Waddle.Core.Runtime
{
    public class RuntimeStack
    {
        private readonly RuntimeValue[] _buffer = new RuntimeValue[1024];
        private int _top;

        public int Count => _top;

        public void Push(RuntimeValue value)
        {
            // TODO check stack overflow
            _buffer[_top++] = value;
        }

        public RuntimeValue Pop()
        {
            // TODO check stack underflow
            return _buffer[--_top];
        }

        public RuntimeValue this[int index]
        {
            get
            {
                if (index < 0 || index >= _buffer.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"index {index} out of range");
                }

                return _buffer[index];
            }
            set
            {
                if (index < 0 || index >= _buffer.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"index {index} out of range");
                }

                _buffer[index] = value;
            }
        }
    }
}