using BIL_Interpreter.Attributes;
using BIL_Interpreter.Helpers;

namespace BIL_Interpreter.Features.Operators;

[LanguageOperator("CALL")]
public class Call(EventEnvironment environment, string argument) : Operator(environment, argument)
{
    internal override void Evaluate()
    {
        Executor.TryExecute(Environment, Argument);
    }
}