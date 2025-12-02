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
        private const string ExitingMsg = "[INFO] Exiting...";

        public string FullName => "Assembler";

        public string Command => "asm";

        public int Run(string[] args, List<string> rootCommand)
        {
            try
            {
                Console.WriteLine("[INFO] Checking arguments...");
                if (args.Length < 1 || args.Contains("-h") || args.Contains("--help"))
                {
                    Console.WriteLine("[INFO] Constructing help...");
                    var hp = HelpPrinter.Create(rootCommand, $"{Command}", FullName)
                        .SetDescription("Assembles the file into an object file(.obj)")
                        .AddUsage("<input> <output>")
                        .AddArgument("output", "The output file path")
                        .AddArgument("input", "The input file path")
                        .AddExample("syslib.asm syslib.obj")
                        .AddExample("syslib.asm")
                        .AddNote("If the name of the output file is not provided, a new file with the same name but with the extension .obj will be generated.");
                    Console.WriteLine("[INFO] Help constructed!");
                    Console.WriteLine("[INFO] Printing help...");
                    hp.Print();
                    Console.WriteLine("[INFO] Help printed!");
                    Console.WriteLine(ExitingMsg);
                    return 0;
                }
                if (args.Length > 2)
                {
                    Console.WriteLine("[ERROR] Too many arguments!");
                    Console.WriteLine(ExitingMsg);
                    return 1;
                }
                Console.WriteLine("[INFO] Arguments are valid!");
                Console.WriteLine("[INFO] Getting input file...");
                string inputfile = args[0];
                Console.WriteLine($"[INFO] Input file: '{inputfile}'");
                Console.WriteLine("[INFO] Checking input file...");
                if (!File.Exists(inputfile))
                {
                    Console.WriteLine($"[ERROR] Input file '{inputfile}' does not exist!");
                    Console.WriteLine(ExitingMsg);
                    return 1;
                }
                Console.WriteLine("[INFO] Input file exists!");
                Console.WriteLine("[INFO] Getting output file...");
                string outputfile = args.Length > 1 ? args[1] : Path.ChangeExtension(inputfile, ".obj");
                Console.WriteLine($"[INFO] Output file: '{outputfile}'");
                Console.WriteLine("[INFO] Reading input file...");
                string file = File.ReadAllText(inputfile);
                Console.WriteLine("[INFO] Input file read!");
                Console.WriteLine($"[INFO] Assembling '{inputfile}'...");
                ObjectFile result = EctcAssembler.Assemble(file);
                Console.WriteLine("[INFO] Assembly complete!");
                Console.WriteLine("[INFO] Converting to bytes...");
                byte[] bytes = result.ToBytes();
                Console.WriteLine("[INFO] Conversion complete!");
                Console.WriteLine($"[INFO] Writing to '{outputfile}'...");
                File.WriteAllBytes(outputfile, bytes);
                Console.WriteLine("[INFO] Writing complete!");
                Console.WriteLine("[INFO] Assembler complete!");
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
