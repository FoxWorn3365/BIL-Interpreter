using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BIL_Interpreter.Attributes;
using BIL_Interpreter.Extensions;
using BIL_Interpreter.Features;
using BIL_Interpreter.Features.Handlers;

namespace BIL_Interpreter.Helpers;

internal static class Executor
{
    private static readonly Dictionary<string, MethodInfo[]> Methods = [];
    
    private static readonly Dictionary<string, Type> SpecialHandlers = [];

    internal static bool TryExecute(FunctionEnvironment environment, string address)
    {
        environment.Environment.Logger?.Debug($"Trying to invoke assembly:function '{address}'");
        
        // Get the stack - we evaluate the whole stack so in case you also have something else please POP the stack
        List<object> extractedStack = environment.ExtractAllStack();
        
        // Try get the method(s) that are represented by the given address
        if (!TryGetMethods(address, out MethodInfo[] methods))
            return false;
        
        // We need to check each method to find the right one - we base our research on arguments count and argument types
        foreach (MethodInfo method in methods)
        {
            environment.Environment.Logger?.Debug($"Evaluating method {method.Name} for invoking with {extractedStack.Count} elements from stack");

            // Get the parameters of the method
            ParameterInfo[] parameters = method.GetParameters();

            object obj = null;

            // If the method is NOT static then the first element of the stack must be the object reference
            if (!method.IsStatic)
            {
                obj = extractedStack[0];
                extractedStack.RemoveAt(0);
            }
                
            // We need to check (as said) the compatibility of this method with our elements from the evaluation stack
            // This function also handles the data conversion and it gives us the arguments to pass - if the function is the right one
            if (!CheckMethodCompatibility(extractedStack.ToArray(), parameters, out object[] args)) 
                continue;
            
            environment.Environment.Logger?.Debug($"Invoking method {method.Name} with {args.Length} arguments and pushing it's return value inside the stack");

            object result = method.Invoke(obj, args);
            
            if (method.ReturnType != typeof(void))
                environment.Stack.Enqueue(result);
            
            return true;
        }

        return false;
    }

    private static bool TryGetMethods(string address, out MethodInfo[] result)
    {
        result = null;

        if (Methods.TryGetValue(address, out MethodInfo[] methods))
        {
            Logger.Instance.Debug($"Trying to hit a cached address ('{address}'), replying with our cache ({methods.Length} methods)");
            
            result = methods;
            return true;
        }
        
        // Try to load special handlers - special handlers are special class that are needed to handle basic language functions
        // Such as Text for strings, List for lists and Dictionary for dictionaries
        // We try to load them only if they weren't loaded (and we keep 'em cached obv)
        if (SpecialHandlers.Count is 0)
            LoadSpecialHandlers();
        
        // An address is composed by <assembly>:<class>.<method>() - the () is optional, and it doesn't get evaluated
        string[] data = address.Split(':');

        if (data.Length != 2)
            return false;
        
        Logger.Instance.Debug($"Trying to find function {data[1]} inside assembly {data[0]}");

        if (data[0] is "Special")
        {
            // If the assembly is 'Special' it means that a special handler is required (and they live in this assembly)
            string[] function = data[1].Split('.');
            
            // The address for special method is just <class name>.<method name> like Text.Concat
            if (function.Length != 2)
                return false;
            
            // Trying to find the special handler that it's required to handle our method
            if (!SpecialHandlers.TryGetValue(function[0], out Type type))
                return false;
            
            // Now that we have the handler we can look for the right method - WE NEED TO CACHE THAT!!!
            MethodInfo targetMethod = type.GetMethod(function[1], BindingFlags.NonPublic | BindingFlags.Static);

            if (targetMethod is null)
                return false;
            
            result = [targetMethod];
            
            return true;
        }
        
        // We need first to check if the required assembly has been registered or not
        if (!AssemblyVault.Assemblies.TryGetValue(data[0], out Assembly assembly))
            return false;

        try
        {
            // We use an external function to parse the signature and get both the class address and the method name
            (string classAddress, string methodName) = ParseMethodSignature(data[1]);
        
            // First we need to find the class (if it exists obv)
            Type type = assembly.GetType(classAddress);
        
            if (type is null)
                return false;
        
            // Now we need to find the right method - be aware that multiple method are possible because of the overload thing
            MethodInfo[] method = type.GetMethods().Where(m => m.Name == methodName).ToArray();
        
            // We just give back every method that might be the right one
            Methods[address] = method;
        
            result = method;
            return true;
        }
        catch (Exception e)
        {
            Logger.Instance.Error(e.ToString());
        }

        return false;
    }
    
    private static (string ClassAddress, string MethodName) ParseMethodSignature(string signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
            throw new ArgumentException();
        
        string cleanSignature = signature.Trim();
        if (cleanSignature.EndsWith("()"))
            cleanSignature = cleanSignature.Substring(0, cleanSignature.Length - 2);
        
        int lastDotIndex = cleanSignature.LastIndexOf('.');
        
        if (lastDotIndex == -1)
            throw new FormatException();
        
        string classAddress = cleanSignature.Substring(0, lastDotIndex);
        string methodName = cleanSignature.Substring(lastDotIndex + 1);

        return (classAddress, methodName);
    }

    // Very important because we works on types here and we also work on giving back the right arguments
    private static bool CheckMethodCompatibility(object[] first, ParameterInfo[] second, out object[] args)
    {
        args = null;

        if (first.Length != second.Length)
        {
            bool found = false;
            for (int i = 0; i < second.Length; i++)
                if (second[i] is not null && second[i].IsDefined(typeof(ParamArrayAttribute), false) && i <= first.Length)
                    found = true;
            
            if (!found)
                return false;
        }

        args = new object[second.Length];

        int infiniteParameter = -1;
        ParameterInfo parameter = null;
        for (int i = 0; i < first.Length; i++)
        {
            object obj = first[i];

            if (infiniteParameter is not -1)
            {
                
                if (args[infiniteParameter] is not List<object> list || parameter is null)
                    throw new Exception("Incorrect parameter - IMPORTANT LOGIC ERROR");
                
                Type singleType = parameter.ParameterType.GetElementType();

                if (singleType == typeof(object))
                {
                    list.Add(obj);
                    goto endParamFixProcess;
                }

                if (singleType != obj.GetType() && !singleType.IsConvertibleTo(obj.GetType()))
                    return false;
            
                if (singleType == obj.GetType())
                    list.Add(obj);
                else
                    list.Add(Converter.TryConvert(obj, singleType));
                
                endParamFixProcess:
                if (i == first.Length - 1)
                    args[infiniteParameter] = list.ToArray();
                
                continue;
            }
            
            parameter = second[i];

            if (parameter.IsOptional && obj is null)
            {
                // We just use the default value
                args[i] = parameter.DefaultValue;
                continue;
            }

            if (Nullable.GetUnderlyingType(parameter.ParameterType) is not null && obj is null)
            {
                // We just put null
                args[i] = null;
                continue;
            }

            if (!parameter.ParameterType.IsValueType && obj is null)
            {
                // We just put null too
                args[i] = null;
                continue;
            }

            if (parameter.IsDefined(typeof(ParamArrayAttribute), false))
            {
                infiniteParameter = i;
                
                Type singleType = parameter.ParameterType.GetElementType();
                
                if (singleType == typeof(object))
                {
                    // Generic
                    args[i] = new List<object> { obj };
                    continue;
                }

                if (singleType != obj.GetType() && !singleType.IsConvertibleTo(obj.GetType()))
                    return false;
            
                if (parameter.ParameterType == obj.GetType())
                    args[i] = new List<object> { obj };
                else
                    args[i] = new List<object> { Converter.TryConvert(obj, singleType) };

                continue;
            }

            if (obj is null)
                return false;

            if (parameter.ParameterType == typeof(object) || parameter.ParameterType.IsObjectOrObjectEnumerable())
            {
                // Generic
                args[i] = obj;
                continue;
            }

            if (parameter.ParameterType != obj.GetType() && !parameter.ParameterType.IsConvertibleTo(obj.GetType()))
                return false;
            
            if (parameter.ParameterType == obj.GetType())
                args[i] = obj;
            else
                args[i] = Converter.TryConvert(obj, parameter.GetType());
        }
        
        return true;
    }

    private static void LoadSpecialHandlers()
    {
        foreach (Type type in typeof(Text).Assembly.GetTypes().Where(t => t.GetCustomAttribute<LanguageSpecialHandler>() != null))
            SpecialHandlers[type.Name] = type;
        
        Logger.Instance.Debug($"Successfully loaded {SpecialHandlers.Count} special handlers!");
    }
}