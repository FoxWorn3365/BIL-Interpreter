using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BIL_Interpreter.Attributes;
using BIL_Interpreter.Extensions;
using BIL_Interpreter.Features;
using BIL_Interpreter.Features.Operators;


namespace BIL_Interpreter.Helpers;

#nullable enable

internal static class Parser
{
    private static readonly Dictionary<string, Type> EnvironmentTypes = new()
    {
        {
            "EVENT",
            typeof(EventEnvironment)
        },
        {
            "PROCEDURE",
            typeof(ProcedureEnvironment)
        }
    };

    private static readonly Dictionary<string, Type> Operators = [];
    
    internal static ProgramEnvironment Parse(string[] code)
    {
        Logger.Instance.Info("Welcome!");
        
        if (Operators.Count is 0)
            LoadOperators();
        
        ProgramEnvironment programEnvironment = new();

        FunctionEnvironment? function = null;
        for (int i = 0; i < code.Length; i++)
        {
            string row = code[i].RemoveInitialSpaces();

            if (row.Length < 2)
                continue;
            
            string[] data = row.Split(' ');

            if (data.Length < 1)
                continue;

            if (data.Length > 2)
                data[1] = string.Join(" ", data.Skip(1));

            switch (function)
            {
                case null when EnvironmentTypes.TryGetValue(data[0], out Type functionType) && data.Length > 1:
                    Logger.Instance.Debug($"Starting definition of function '{data[1]}'");
                    function = Activator.CreateInstance(functionType, data[1], programEnvironment, null, null) as FunctionEnvironment;
                    continue;
                case not null when data[0] is "END":
                    Logger.Instance.Debug($"Definition of function finished - parsed and cached {function.Stack.Count} orders");
                    switch (function)
                    {
                        case ProcedureEnvironment procedure:
                            programEnvironment.Procedures.Add(procedure.Name, procedure);
                            break;
                        case EventEnvironment eventFunction when programEnvironment.Events.ContainsKey(eventFunction.Name):
                            programEnvironment.Events[eventFunction.Name].Add(eventFunction);
                            break;
                        case EventEnvironment eventFunction:
                            programEnvironment.Events[eventFunction.Name] = [eventFunction];
                            break;
                    }
                    
                    function = null;
                    continue;
                case not null when data[0] is "LABEL" && data.Length > 1:
                    Logger.Instance.Debug($"Definition of label '{data[1]}' for function {function.Name}");
                    function.Labels.Add(data[1], i);
                    continue;
                case null:
                    continue;
            }
            
            Logger.Instance.Debug($"Parsing instruction {data[0]} for function {function.Name}");

            if (!Operators.TryGetValue(data[0], out Type operatorType)) 
                continue;
            
            string args = data.Length > 1 ? data[1] : string.Empty;

            if (Activator.CreateInstance(operatorType, [function, args]) is not Operator @operator)
                continue;
                
            function.Instructions.Add(@operator);
        }
        
        return programEnvironment;
    }
    
    private static void LoadOperators()
    {
        foreach (Type type in typeof(Operator).Assembly.GetTypes().Where(t => t.GetCustomAttribute<LanguageOperator>() != null && !t.IsAbstract))
        {
            LanguageOperator attribute = type.GetCustomAttribute<LanguageOperator>();

            Operators[attribute.Name] = type;
        }
    }
}