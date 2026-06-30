namespace BIL_Interpreter.Extensions;

public static class StringExtension
{
    public static string RemoveInitialSpaces(this string str)
    {
        while (str.Length > 1 && str[0] is ' ')
            str = str.Substring(1);

        return str;
    }
}