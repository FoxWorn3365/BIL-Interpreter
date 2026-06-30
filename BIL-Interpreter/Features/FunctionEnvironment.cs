using System.Collections.Generic;
using BIL_Interpreter.Enums;
using BIL_Interpreter.Features.Operators;
using BIL_Interpreter.Helpers;

namespace BIL_Interpreter.Features;

public abstract class FunctionEnvironment(string name, ProgramEnvironment programEnvironment, List<Operator> instructions = null, Dictionary<string, int> labels = null)
{
    /// <summary>
    /// Gets the <see cref="EnvironmentType"/> of the current Function
    /// </summary>
    public abstract EnvironmentType Type { get; }
    
    /// <summary>
    /// Gets the name of the current Function
    /// </summary>
    public string Name { get; } = name;
    
    /// <summary>
    /// Gets or sets the cursor position (for code execution) of the current Function
    /// </summary>
    internal int CursorPosition { get; set; } = 0;

    /// <summary>
    /// Gets the cached instructions of this Function
    /// </summary>
    internal List<Operator> Instructions { get; } = instructions ?? [];

    /// <summary>
    /// Gets the label list for this Function
    /// </summary>
    internal Dictionary<string, int> Labels { get; } = labels ?? [];
    
    /// <summary>
    /// Gets the parent <see cref="ProgramEnvironment"/> for this Function
    /// </summary>
    internal ProgramEnvironment Environment { get; } = programEnvironment;

    /// <summary>
    /// Gets or sets the local variables for the current Function
    /// </summary>
    internal Dictionary<string, object> Variables { get; private set; } = [];

    /// <summary>
    /// Gets the evaluation stack for this Function
    /// </summary>
    internal Queue<object> Stack { get; } = [];

    /// <summary>
    /// Execute the current Function
    /// </summary>
    /// <param name="variables"></param>
    /// <returns></returns>
    public Dictionary<string,object> Execute(Dictionary<string,object> variables = null)
    {
        Variables = variables ?? [];
        
        Environment.Logger?.Debug($"Executing function {Name}");

        for (CursorPosition = 0; CursorPosition < Instructions.Count; CursorPosition++)
        {
            Environment.Logger?.Debug($"Executing instruction {Instructions[CursorPosition]} (CursorPosition: {CursorPosition})");
            Instructions[CursorPosition].Evaluate();
        }
        
        Environment.Logger?.Debug($"Finished executing function {Name}");

        return Variables;
    }

    internal List<object> ExtractAllStack()
    {
        List<object> extractedStack = [];
        
        while (Stack.Count > 0)
            extractedStack.Add(Stack.Dequeue());
        
        return extractedStack;
    }
}