using System;
using System.Collections.Generic;
using BIL_Interpreter.Attributes;
using BIL_Interpreter.Exceptions;

namespace BIL_Interpreter.Features.Handlers;

[LanguageSpecialHandler]
internal static class Dictionary
{
    internal static readonly HashSet<object> InternalDictionaries = [];
    
    private static readonly Random SystemRandom = new();

    internal static object Create(params object[] keyValuePairs)
    {
        // The input must be a pair
        if (keyValuePairs.Length % 2 is not 0)
            throw new BadArgumentTypeException();

        Dictionary<object, object> dictionary = [];
        for (int i = 0; i < keyValuePairs.Length; i += 2)
            dictionary[keyValuePairs[i]] = keyValuePairs[i + 1];
        
        InternalDictionaries.Add(dictionary);
        return dictionary;
    }

    internal static object Get(object dictionary, object key)
    {
        if (dictionary is not Dictionary<object, object> castedDictionary)
            throw new BadArgumentTypeException();
        
        return castedDictionary.TryGetValue(key, out object value) ? value : null;
    }
    
    internal static void Set(object dictionary, object key, object value)
    {
        if (dictionary is not Dictionary<object, object> castedDictionary)
            throw new BadArgumentTypeException();
        
        castedDictionary[key] = value;
    }

    internal static void Remove(object dictionary, object key)
    {
        if (dictionary is not Dictionary<object, object> castedDictionary)
            throw new BadArgumentTypeException();
        
        castedDictionary.Remove(key);
    }

    internal static object GetKeys(object dictionary)
    {
        return dictionary is not Dictionary<object, object> castedDictionary ? throw new BadArgumentTypeException() : castedDictionary.Keys;
    }
    
    internal static object GetValues(object dictionary)
    {
        return dictionary is not Dictionary<object, object> castedDictionary ? throw new BadArgumentTypeException() : castedDictionary.Values;
    }

    internal static bool Contains(object dictionary, object key)
    {
        return dictionary is not Dictionary<object, object> castedDictionary ? throw new BadArgumentTypeException() : castedDictionary.ContainsKey(key);
    }

    internal static int Count(object dictionary)
    {
        return dictionary is not Dictionary<object, object> castedDictionary ? throw new BadArgumentTypeException() : castedDictionary.Count;
    }

    internal static object Clone(object dictionary)
    {
        if (dictionary is not Dictionary<object, object> castedDictionary)
            throw new BadArgumentTypeException();
        
        Dictionary<object, object> clone = [];

        foreach (KeyValuePair<object, object> pair in castedDictionary)
            clone[pair.Key] = pair.Value;
        
        return clone;
    }

    internal static object Merge(object dictionary, object dictionary2)
    {
        if (dictionary is not Dictionary<object, object> castedDictionary || dictionary2 is not Dictionary<object, object> castedDictionary2)
            throw new BadArgumentTypeException();

        foreach (KeyValuePair<object, object> kvp in castedDictionary2)
            castedDictionary[kvp.Key] = kvp.Value;
        
        return castedDictionary;
    }
}