using System;

namespace BIL_Interpreter.Exceptions;

public class BadCollectionException(Type type) : Exception
{
    private Type Type { get; } = type;
}