using System;

namespace BIL_Interpreter.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class LanguageSpecialHandler() : Attribute
{ }