using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ectc.Assembler;
using Ectc.Dto;

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
            args = new string[] { "asm", "prog.txt" };
#endif
            ITool[] tools = new ITool[] { new AssemblerTool() };
            return new ListTools("tools", "Tools", tools).Run(args, new System.Collections.Generic.List<string>());
        }
    }

    public class AssemblerTool : ITool
    {
        public string FullName => "Assembler";

        public string Command => "asm";

        public int Run(string[] args, List<string> rootCommand)
        {
            Console.WriteLine("Testing arguments...");
            if (args.Length < 1 || args.Contains("-h") || args.Contains("--help"))
            {
                HelpPrinter.Create(rootCommand, $"{Command}", FullName)
                    .SetDescription("Assembles the file into an object file(.obj)")
                    .AddUsage("<input> <output>")
                    .AddArgument("output", "The output file path")
                    .AddArgument("input", "The input file path")
                    .AddExample("syslib.asm syslib.obj")
                    .AddExample("syslib.asm")
                    .AddNote("If the name of the output file is not provided, a new file with the same name but with the extension .obj will be generated.")
                    .Print();
                return 0;
            }
            if (args.Length > 2)
            {
                Console.WriteLine("Too many arguments!");
                return 1;
            }

            string inputfile = args[0];
            Console.WriteLine($"Assembling {inputfile}...");
            string outputfile = args.Length > 1 ? args[1] : Path.ChangeExtension(inputfile, ".obj");
            Console.WriteLine($"Output file: {outputfile}");
            ObjectFile result = EctcAssembler.Assemble(File.ReadAllText(inputfile));
            ushort[] words = result.ToWords();
            byte[] bytes = words.SelectMany(w => BitConverter.GetBytes(w)).ToArray();
            Console.WriteLine($"Writing to {outputfile}...");
            File.WriteAllBytes(outputfile, bytes);
            Console.WriteLine("Done!");

            return 0;
        }
    }
}
