using BIL_Interpreter.Attributes;
using BIL_Interpreter.Helpers;

namespace BIL_Interpreter.Features.Operators;

[LanguageOperator("EQ")]
public class Eq(EventEnvironment environment, string argument) : Operator(environment, argument)
{
    internal override void Evaluate()
    {
        // Get two elements
        object first = Environment.Stack.Dequeue();
        object second = Environment.Stack.Dequeue();
        
        bool equals = first is not null && first.Equals(second);
        
        Environment.Stack.Enqueue(equals);
    }
}