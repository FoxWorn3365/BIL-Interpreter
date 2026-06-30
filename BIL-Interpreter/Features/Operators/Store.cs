using System.Reflection;
using BIL_Interpreter.Attributes;
using BIL_Interpreter.Helpers;

namespace BIL_Interpreter.Features.Operators;

[LanguageOperator("STORE")]
public class Store(EventEnvironment environment, string argument) : Operator(environment, argument)
{
    internal override void Evaluate()
    {
        object value = Environment.Stack.Dequeue();
        
        if (Argument.Contains("local:"))
            Environment.Variables[Argument.Replace("local:", "")] = value;
        else
            Environment.Environment.Variables[Argument] = value;
    }
}