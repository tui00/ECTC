using System;
using System.Collections.Generic;
using System.Linq;

namespace Ectc.Tools
{
    public class ListTools : ITool
    {
        public string FullName { get; private set; }
        public string Command { get; private set; }

        private readonly ITool[] avalableTools;

        public ListTools(string command, string fullName, ITool[] avalableTools)
        {
            Command = command;
            FullName = fullName;
            this.avalableTools = avalableTools;
        }

        public int Run(string[] args, List<string> rootCommand)
        {
            if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
            {
                var hp = HelpPrinter.Create(rootCommand, $"{Command}", FullName)
                    .SetDescription("Tools for ECTC")
                    .SetDescriptionColumn(10)
                    .AddUsage("<tool> [args]")
                    .StartCustomSection("Tools");
                if (avalableTools.Length == 0)
                {
                    hp.AddItem("No tools avalable", "");
                }
                else
                {
                    foreach (var tool in avalableTools)
                    {
                        hp.AddItem(tool.Command, tool.FullName);
                    }
                }
                hp.Print();
                return 0;
            }

            foreach (var tool in avalableTools)
            {
                if (tool.Command == args[0])
                {
                    rootCommand.Add(Command);
                    return tool.Run(args.Skip(1).ToArray(), rootCommand);
                }
            }
            Console.WriteLine($"Tool '{args[0]}' not found!");
            return 1;
        }
    }
}
