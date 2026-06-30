using System;

namespace BIL_Interpreter.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class LanguageOperator(string name) : Attribute
    {
        internal string Name { get; } = name;
    }
}