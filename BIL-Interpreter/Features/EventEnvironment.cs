using System.Collections.Generic;
using BIL_Interpreter.Enums;
using BIL_Interpreter.Features.Operators;

namespace BIL_Interpreter.Features;

public class EventEnvironment(string name, ProgramEnvironment programEnvironment, List<Operator> instructions = null, Dictionary<string, int> labels = null) : FunctionEnvironment(name, programEnvironment, instructions, labels)
{
    public override EnvironmentType Type { get; } = EnvironmentType.Event;
}