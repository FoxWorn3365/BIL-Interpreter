using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace BIL_Interpreter.Extensions;

internal static class CollectionExtension
{
    internal static bool TryAddSequential(this object collection, object element)
    {
        if (collection is null) 
            return false;
        
        if (collection is IList list)
        {
            if (list.IsReadOnly || list.IsFixedSize) 
                return false;
            
            list.Add(element);
            return true;
        }

        Type type = collection.GetType();
        
        foreach (Type iface in type.GetInterfaces())
        {
            if (!iface.IsGenericType || iface.GetGenericTypeDefinition() != typeof(ICollection<>)) 
                continue;
            
            try
            {
                MethodInfo addMethod = iface.GetMethod("Add");
                addMethod?.Invoke(collection, [element]);
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        string[] sequentialMethods = ["Enqueue", "Push"];
        foreach (string methodName in sequentialMethods)
        {
            MethodInfo method = type.GetMethod(methodName);
            
            if (method is null || method.GetParameters().Length != 1) 
                continue;
            
            try
            {
                method.Invoke(collection, [element]);
                return true;
            }
            catch
            {
                continue; 
            }
        }
        
        return false;
    }
    
    internal static bool TrySetSequential(this object collection, object index, object value)
    {
        if (collection is null || index is not int targetIndex || targetIndex < 0)
            return false;

        if (collection is Array array)
        {
            if (targetIndex >= array.Length) 
                return false;
            
            try
            {
                array.SetValue(value, targetIndex);
                return true;
            }
            catch
            {
                return false; 
            }
        }

        if (collection is not IList list) 
            return false;
        
        if (list.IsReadOnly || targetIndex >= list.Count) 
            return false;
        
        try
        {
            list[targetIndex] = value;
            return true;
        }
        catch
        {
            return false; 
        }
    }
    
    internal static bool TryRemoveAtSequential(this object collection, object index)
    {
        if (collection is null || index is not int targetIndex || targetIndex < 0)
            return false;
        
        if (collection is Array array)
        {
            if (targetIndex >= array.Length) 
                return false;
            
            Type elementType = array.GetType().GetElementType();
            object defaultValue = elementType?.IsValueType ?? false ? Activator.CreateInstance(elementType) : null;
            
            array.SetValue(defaultValue, targetIndex);
            
            return true;
        }

        if (collection is not IList list) 
            return false;
        
        if (list.IsReadOnly || targetIndex >= list.Count) 
            return false;
            
        list.RemoveAt(targetIndex);
            
        return true;

    }
}