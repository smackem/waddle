namespace Waddle.Core.Syntax.Ast
{
    public class Visitor
    {
        abstract class AttributeValue
        {
            public abstract TResult Accept<TState, TResult>(IAttributeValueVisitor<TState, TResult> visitor, TState state);
        }

        interface IAttributeValueVisitor<TState, TResult>
        {
            TResult Visit(IntegerValue v, TState state);
            TResult Visit(StringValue v, TState state);
            TResult Visit(BoolValue v, TState state);
            TResult Visit(DecimalValue v, TState state);
        }

        class IntegerValue : AttributeValue
        {
            private int Value { get;  }

            public IntegerValue(int value)
            {
                Value = value;
            }

            public override TResult Accept<TState, TResult>(IAttributeValueVisitor<TState, TResult> visitor, TState state)
            {
                return visitor.Visit(this, state);
            }
        }

        class StringValue : AttributeValue
        {
            private string Value { get;  }

            public StringValue(string value)
            {
                Value = value;
            }
            
            public override TResult Accept<TState, TResult>(IAttributeValueVisitor<TState, TResult> visitor, TState state)
            {
                return visitor.Visit(this, state);
            }
        }

        class BoolValue : AttributeValue
        {
            private bool Value { get;  }

            public BoolValue(bool value)
            {
                Value = value;
            }
            
            public override TResult Accept<TState, TResult>(IAttributeValueVisitor<TState, TResult> visitor, TState state)
            {
                return visitor.Visit(this, state);
            }
        }

        class DecimalValue : AttributeValue
        {
            public override TResult Accept<TState, TResult>(IAttributeValueVisitor<TState, TResult> visitor, TState state)
            {
                return visitor.Visit(this, state);
            }
        }

        class Consumer
        {
            void Consume(AttributeValue v)
            {
                var str = v.Accept(new PrintVisitor(), null);
            }

            class PrintVisitor : IAttributeValueVisitor<object, string>
            {
                public string Visit(IntegerValue v, object state)
                {
                    return v.ToString();
                }

                public string Visit(StringValue v, object state)
                {
                    return v.ToString();
                }

                public string Visit(BoolValue v, object state)
                {
                    return "on";
                }

                public string Visit(DecimalValue v, object state)
                {
                    return "decimal";
                }
            }
        }
    }
}