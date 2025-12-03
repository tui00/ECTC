using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ectc.Dto;
using Ectc.Linker;

namespace Ectc.Tools
{
    public class LinkerTool : ITool
    {
        private const string ExitingMsg = "Exiting...";

        public string FullName => "Linker";
        public string Command => "link";

        public int Run(string[] args, List<string> rootCommand)
        {
            try
            {
                if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
                {
                    HelpPrinter.Create(rootCommand, $"{Command}", FullName)
                        .SetDescription("Linking the file into an bytes (.bin)")
                        .AddUsage("<input ..> <output>")
                        .AddArgument("input ..", "The input file paths")
                        .AddArgument("output", "The output file path")
                        .AddExample("syslib.bin syslib.obj")
                        .AddExample("out.bin prog.obj syslib.obj")
                        .AddNote("If no output file is specified, the name of the first input file will be used with the extension changed to .bin")
                        .Print();
                    return 0;
                }
                string outputfile = args[args.Length - 1];
                string[] inputfiles = args.Take(args.Length - 1).ToArray();
                if (args.Length == 1)
                    outputfile = Path.ChangeExtension(inputfiles[0], ".bin");
                if (!inputfiles.All(File.Exists))
                {
                    Console.WriteLine($"Input file(s) '{string.Join(", ", inputfiles.Where(inputfile => !File.Exists(inputfile)))}' do(es) not exist!");
                    Console.WriteLine(ExitingMsg);
                    return 1;
                }
                List<ObjectFile> objectFiles = new List<ObjectFile>(inputfiles.Length);
                foreach (string inputfile in inputfiles)
                    objectFiles.Add(ObjectFile.FromBytes(File.ReadAllBytes(inputfile)));
                byte[] bytes = EctcLinker.Link(objectFiles.ToArray());
                File.WriteAllBytes(outputfile, bytes);
                Console.WriteLine(ExitingMsg);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
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
