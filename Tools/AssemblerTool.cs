using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ectc.Assembler;
using Ectc.Dto;

namespace Ectc.Tools
{
    public class AssemblerTool : ITool
    {
        private const string ExitingMsg = "Exiting...";

        public string FullName => "Assembler";

        public string Command => "asm";

        public int Run(string[] args, List<string> rootCommand)
        {
            try
            {
                if (args.Length < 1 || args.Contains("-h") || args.Contains("--help"))
                {
                    HelpPrinter.Create(rootCommand, $"{Command}", FullName)
                        .SetDescription("Assembles the file into an object file(.obj)")
                        .AddUsage("<input> [<output>]")
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
                    Console.WriteLine(ExitingMsg);
                    return 1;
                }
                string inputfile = args[0];
                if (!File.Exists(inputfile))
                {
                    Console.WriteLine($"Input file '{inputfile}' does not exist!");
                    Console.WriteLine(ExitingMsg);
                    return 1;
                }
                string outputfile = args.Length > 1 ? args[1] : Path.ChangeExtension(inputfile, ".obj");
                Console.WriteLine($"Output file: '{outputfile}'");
                string file = File.ReadAllText(inputfile);
                ObjectFile result = EctcAssembler.Assemble(file);
                byte[] bytes = result.ToBytes();
                File.WriteAllBytes(outputfile, bytes);
                Console.WriteLine(ExitingMsg);
                return 0;
            }
            catch (Exception e)
            {
                if (e.Data.Contains("LineNumber"))
                    Console.WriteLine($"[ERROR] Line {e.Data["LineNumber"]}: {e.Message}");
                else
                    Console.WriteLine($"[ERROR] {e.Message}");
                Console.WriteLine(ExitingMsg);
#if DEBUG
                throw;
#else
                return 1;
#endif
            }
        }
    }
}
