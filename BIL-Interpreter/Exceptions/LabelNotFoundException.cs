using System;
using BIL_Interpreter.Features;

namespace BIL_Interpreter.Exceptions;

public class LabelNotFoundException(FunctionEnvironment environment, string labelName) : Exception
{
    private FunctionEnvironment Environment { get; } = environment;
    
    private string LabelName { get; } = labelName;
}