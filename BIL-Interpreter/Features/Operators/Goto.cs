using BIL_Interpreter.Attributes;
using BIL_Interpreter.Exceptions;
using BIL_Interpreter.Helpers;

namespace BIL_Interpreter.Features.Operators;

[LanguageOperator("GOTO")]
public class Goto(EventEnvironment environment, string argument) : Operator(environment, argument)
{
    internal override void Evaluate()
    {
        if (Environment.Labels.TryGetValue(Argument, out int index))
            Environment.CursorPosition = index;
        else
            throw new LabelNotFoundException(Environment, Argument);
    }
}