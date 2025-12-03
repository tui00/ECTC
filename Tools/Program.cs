using System;
using System.Collections.Concurrent;

namespace Ectc.Tools
{
    /// <summary>
    /// Provides the entry point for the application.
    /// </summary>
    internal static class Program
    {
#if DEBUG
        private static bool runned = false;
#endif

        /// <summary>
        /// The main entry point method for the application.
        /// </summary>
        /// <param name="args">The command-line arguments provided to the application.</param>
        /// <returns>
        /// An integer exit code indicating the success or failure of the application execution.
        /// A return value of 0 typically indicates success, while non-zero values indicate errors.
        /// </returns>
        static int Main(string[] args)
        {
#if DEBUG
            if (!runned)
            {
                runned = true;
                Main(new string[] { "asm", "prog.txt" });
                Main(new string[] { "link", "prog.obj", "prog.bin" });
                return 0;
            }
#endif
            ITool[] tools = new ITool[] { new AssemblerTool(), new LinkerTool() };
            return new ListTools("tools", "Tools", tools).Run(args, new System.Collections.Generic.List<string>());
        }
    }
}
