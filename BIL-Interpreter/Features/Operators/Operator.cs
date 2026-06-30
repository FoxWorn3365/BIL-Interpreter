namespace BIL_Interpreter.Features.Operators
{
    public abstract class Operator(EventEnvironment environment, string argument)
    {
        protected EventEnvironment Environment { get; } = environment;
        
        protected string Argument { get; } = argument;
        
        internal virtual void Evaluate()
        { }
    }
}