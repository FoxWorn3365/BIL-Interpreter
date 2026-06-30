using System;

namespace BIL_Interpreter.Helpers;

internal static class Converter
{
    internal static object TryConvert(object obj, Type type)
    {
        if (obj.GetType() == type)
            return obj;

        if (type.IsEnum)
            return Enum.Parse(type, Convert.ToString(obj), true);
        
        return Convert.ChangeType(obj, type);
    }
}