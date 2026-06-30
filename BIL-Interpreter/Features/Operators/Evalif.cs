using BIL_Interpreter.Attributes;
using BIL_Interpreter.Exceptions;

namespace BIL_Interpreter.Features.Operators;

[LanguageOperator("EVALIF")]
public class Evalif(EventEnvironment environment, string argument) : Operator(environment, argument)
{
    internal override void Evaluate()
    {
        if (Environment.Stack.Dequeue() is not false) 
            return;
        
        if (Environment.Labels.TryGetValue(Argument, out var label))
            Environment.CursorPosition = label;
        else
            throw new LabelNotFoundException(Environment, Argument);
    }
}