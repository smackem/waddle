namespace Waddle.Core.Runtime
{
    public record RuntimeValue
    {
        public int Integer { get; }
        public string? String { get; }

        public RuntimeValue(int integer)
        {
            Integer = integer;
            String = null;
        }

        public RuntimeValue(string str)
        {
            Integer = 0;
            String = str;
        }
    }
}