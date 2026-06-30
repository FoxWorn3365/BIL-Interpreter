using System;
using System.Linq;
using BIL_Interpreter.Attributes;
using BIL_Interpreter.Helpers;

namespace BIL_Interpreter.Features.Operators;

[LanguageOperator("PUSH")]
public class Push(EventEnvironment environment, string argument) : Operator(environment, argument)
{
    internal override void Evaluate()
    {
        if (Argument.Length is 0)
            return;

        switch (Argument)
        {
            case "true":
                Environment.Stack.Enqueue(true);
                Environment.Environment.Logger?.Debug($"Inserted (pushed) {Environment.Stack.Peek()} ({Argument})");
                return;
            case "false":
                Environment.Stack.Enqueue(false);
                Environment.Environment.Logger?.Debug($"Inserted (pushed) {Environment.Stack.Peek()} ({Argument})");
                return;
            case "null":
                Environment.Stack.Enqueue(null);
                Environment.Environment.Logger?.Debug($"Inserted (pushed) {Environment.Stack.Peek()} ({Argument})");
                return;
        }

        if (Argument[0] is '"')
        {
            // Load string
            Environment.Stack.Enqueue(new string(Argument.ToCharArray().Skip(1).Take(Argument.Length - 2).ToArray()));
            Environment.Environment.Logger?.Debug($"Inserted (pushed) {Environment.Stack.Peek()} ({Argument})");
            return;
        }

        if (Argument.Contains("."))
        {
            // Load float (single)
            Environment.Stack.Enqueue(Convert.ToSingle(Argument));
            Environment.Environment.Logger?.Debug($"Inserted (pushed) {Environment.Stack.Peek()} ({Argument})");
            return;
        }
        
        // Load int (int32)
        Environment.Stack.Enqueue(Convert.ToInt32(Argument));
        Environment.Environment.Logger?.Debug($"Inserted (pushed) {Environment.Stack.Peek()} ({Argument})");
    }
}