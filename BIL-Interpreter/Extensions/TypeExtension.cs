using System;
using System.Collections;

namespace BIL_Interpreter.Extensions;

public static class TypeExtension
{
    public static bool IsConvertibleTo(this Type fromType, Type toType)
    {
        if (typeof(IConvertible).IsAssignableFrom(fromType))
        {
            try
            {
                object defaultValue = Activator.CreateInstance(fromType);
                _ = Convert.ChangeType(defaultValue, toType);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        return toType.IsAssignableFrom(fromType);
    }
    
    public static bool IsObjectOrObjectEnumerable(this Type type)
    {
        if (type == typeof(object))
            return true;
        
        if (type.IsArray && type.GetElementType() == typeof(object))
            return true;
        
        if (type.IsGenericType)
        {
            foreach (var arg in type.GetGenericArguments())
            {
                if (arg == typeof(object))
                    return true;
            }
        }
        
        if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
            return true; 

        return false;
    }
}