using System;
using System.Collections.Generic;
using System.Linq;
using BIL_Interpreter.Exceptions;

namespace BIL_Interpreter.Extensions;

public static class EnumerableExtension
{
    public static IEnumerable<Type> ExtractTypes<T>(this IEnumerable<T> types) where T : class
    {
        return types.Select(type => type.GetType());
    }

    public static List<T> ExtractFromStack<T>(this Queue<T> stack, byte amount)
    {
        List<T> elements = [];
        for (byte i = 0; i < stack.Count && i < amount; i++)
            elements.Add(stack.Dequeue());
        
        return elements;
    }

    public static List<object> CastAsObject<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Cast<object>().ToList();
    }
    
    public static bool IsGenericIEnumerable(this object obj)
    {
        if (obj is null) 
            return false;

        Type tipoIntero = obj.GetType();
        
        if (tipoIntero.IsGenericType && tipoIntero.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return true;
        
        return tipoIntero.GetInterfaces().Any(interfaccia => interfaccia.IsGenericType && interfaccia.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }
    
    public static List<object> ConvertToListOfObject(this object obj)
    {
        if (!obj.IsGenericIEnumerable())
            throw new BadArgumentTypeException();
        
        List<object> risultato = [];

        var collezioneClassica = (System.Collections.IEnumerable)obj;

        risultato.AddRange(collezioneClassica.Cast<object>());

        return risultato;
    }
}