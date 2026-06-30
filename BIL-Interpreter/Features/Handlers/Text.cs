using System;
using System.Collections.Generic;
using BIL_Interpreter.Attributes;
using BIL_Interpreter.Enums;
using BIL_Interpreter.Exceptions;

namespace BIL_Interpreter.Features.Handlers;

[LanguageSpecialHandler]
internal static class Text
{
    internal static string Concat(params object[] elements)
    {
        string result = string.Empty;
        foreach (object obj in elements)
            if (obj is string str)
                result += str;
            else
                result += obj.ToString();
        
        return result;
    }

    internal static int Length(object element)
    {
        if (element is string str)
            return str.Length;
        
        return element.ToString().Length;
    }

    internal static bool IsEmpty(object element)
    {
        if (element is not string str)
            throw new BadArgumentTypeException();

        return str.Length == 0;
    }

    internal static string ToUpper(object element)
    {
        if (element is not string str)
            throw new BadArgumentTypeException();

        return str.ToUpper();
    }

    internal static string ToLower(object element)
    {
        if (element is not string str)
            throw new BadArgumentTypeException();
        
        return str.ToLower();
    }

    internal static bool Contains(object element, object value)
    {
        if (element is not string str || value is not string val)
            throw new BadArgumentTypeException();
        
        return str.Contains(val);
    }

    internal static List<string> Split(object element, object separator)
    {
        if (element is not string str || separator is not string sep)
            throw new BadArgumentTypeException();

        return [..str.Split(sep.ToCharArray())];
    }

    internal static string Segment(object element, object start, object length = null)
    {
        if (element is not string str)
            throw new BadArgumentTypeException();
        
        int startInt = Convert.ToInt32(start);
        int lengthInt = Convert.ToInt32(length ?? str.Length);

        return str.Substring(startInt, lengthInt);
    }

    internal static string Replace(object element, object oldValue, object newValue)
    {
        if (element is not string str || oldValue is not string old || newValue is not string newV)
            throw new BadArgumentTypeException();
            
        return str.Replace(old, newV);
    }
}