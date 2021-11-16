using Waddle.Core.ByteCode;

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

        public RuntimeValue Get(int index)
        {
            // TODO check index
            return _buffer[index];
        }

        public void Set(int index, RuntimeValue value)
        {
            // TODO check index
            _buffer[index] = value;
        }
    }
}