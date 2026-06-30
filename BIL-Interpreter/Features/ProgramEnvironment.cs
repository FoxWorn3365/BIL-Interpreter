using System;
using System.Collections.Generic;
using System.IO;
using BIL_Interpreter.Helpers;

#nullable enable

namespace BIL_Interpreter.Features
{
    public class ProgramEnvironment
    {
        private static Random Random { get; } = new();
        
        /// <summary>
        /// Gets the id of this ProgramEnvironment
        /// </summary>
        public string Id { get; }
        
        /// <summary>
        /// Gets the list of the registered <see cref="ProcedureEnvironment"/> and their names
        /// </summary>
        public Dictionary<string, ProcedureEnvironment> Procedures { get; } = [];
        
        /// <summary>
        /// Gets the list of <see cref="EventEnvironment"/> and their names
        /// </summary>
        public Dictionary<string, List<EventEnvironment>> Events { get; } = [];
        
        /// <summary>
        /// Gets the variables for this ProgramEnvironment
        /// </summary>
        public Dictionary<string, object> Variables { get; } = new()
        {
            {
                "special:random",
                Random
            }
        };
        
        /// <summary>
        /// Gets or sets whether the debug mode is enabled or not for the current ProgramEnvironment
        /// </summary>
        public bool Debug { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the file log directory for the current ProgramEnvironment.
        /// To disable logs you can set it to null
        /// </summary>
        public string? LogFile { get; set; }

        /// <summary>
        /// Gets or sets the dedicated <see cref="Logger"/> instance for this ProgramEnvironment
        /// </summary>
        internal Logger? Logger { get; private set; } = null;

        /// <summary>
        /// Create a new instance of <see cref="ProgramEnvironment"/> from code contained in a <see cref="string[]"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ProgramEnvironment Create(string[] input)
        {
            return Parser.Parse(input);
        }

        internal ProgramEnvironment()
        {
            Id = Guid.NewGuid().ToString();
            LogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".bil_env_{Id}.log");
            
            if (LogFile is not null)
                Logger = new(this);
        }
    }
}