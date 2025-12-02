namespace Ectc.Tools
{
    /// <summary>
    /// Provides the entry point for the application.
    /// </summary>
    internal static class Program
    {
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
            args = new string[] { "link", "prog.bin", "prog.obj" };
#endif
            ITool[] tools = new ITool[] { new AssemblerTool(), new LinkerTool() };
            return new ListTools("tools", "Tools", tools).Run(args, new System.Collections.Generic.List<string>());
        }
    }
}
