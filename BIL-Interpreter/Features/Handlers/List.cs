using System;
using System.Collections.Generic;
using BIL_Interpreter.Attributes;
using BIL_Interpreter.Exceptions;
using BIL_Interpreter.Extensions;

namespace BIL_Interpreter.Features.Handlers;

[LanguageSpecialHandler]
internal static class List
{
    private static readonly Random SystemRandom = new();
    
    internal static List<object> Create(params object[] args)
    {
        List<object> list = [];
        
        foreach (object arg in args)
            list.Add(arg);
        
        return list;
    }

    internal static bool Contains(object list, object arg)
    {
        if (!list.IsGenericIEnumerable())
            throw new BadArgumentTypeException();

        List<object> realList = list.ConvertToListOfObject();
        
        return realList.Contains(arg);
    }

    internal static object Random(object list)
    {
        if (!list.IsGenericIEnumerable())
            throw new BadArgumentTypeException();

        List<object> realList = list.ConvertToListOfObject();

        if (realList.Count is 0)
            return null;
        
        return realList[SystemRandom.Next(0, realList.Count)];
    }
    
    internal static int IndexOf(object list, object element)
    {
        if (!list.IsGenericIEnumerable())
            throw new BadArgumentTypeException();

        List<object> realList = list.ConvertToListOfObject();

        return realList.IndexOf(element);
    }
    
    internal static object Get(object list, object index)
    {
        int indexInt = Convert.ToInt32(index);
        
        if (!list.IsGenericIEnumerable())
            throw new BadArgumentTypeException();

        List<object> realList = list.ConvertToListOfObject();

        return realList[indexInt];
    }

    internal static void Insert(object list, object index, object element)
    {
        int indexInt = Convert.ToInt32(index);
        
        if (!list.IsGenericIEnumerable())
            throw new BadArgumentTypeException();

        List<object> realList = list.ConvertToListOfObject();

        realList.Insert(indexInt, element);
    }

    internal static void Remove(object list, object index)
    {
        int indexInt = Convert.ToInt32(index);
        
        if (!list.IsGenericIEnumerable())
            throw new BadArgumentTypeException();
        
        List<object> realList = list.ConvertToListOfObject();
        
        realList.RemoveAt(indexInt);
    }
    
    internal static void Replace(object list, object index, object element)
    {
        int indexInt = Convert.ToInt32(index);
        
        if (!list.IsGenericIEnumerable())
            throw new BadArgumentTypeException();
        
        List<object> realList = list.ConvertToListOfObject();
        
        realList[indexInt] = element;
    }
}