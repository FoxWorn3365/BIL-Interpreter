using BIL_Interpreter.Attributes;

namespace BIL_Interpreter.Features.Operators;

[LanguageOperator("LOAD")]
public class Load(EventEnvironment environment, string argument) : Operator(environment, argument)
{
    internal override void Evaluate()
    {
        Environment.Stack.Enqueue(Argument.Contains("local:") ? Environment.Variables[Argument.Replace("local:", "")] : Environment.Environment.Variables[Argument]);
    }
}