using System;
using System.Collections.Generic;

namespace Ectc.Tools
{
    internal class HelpPrinter
    {
        private List<string> rootCommand = new List<string>();
        private string name = "";
        private string fullName = "";
        private string description = "";
        private readonly List<string> usages = new List<string> { };
        private readonly List<string> examples = new List<string> { };
        private readonly List<string> arguments = new List<string> { };
        private readonly List<string> notes = new List<string> { };
        private readonly Dictionary<string, List<string>> nonStandartSections = new Dictionary<string, List<string>> { };
        private string lastSection = "";
        private int indent = 2;
        private int descColumn = 30;

        public static HelpPrinter Create(List<string> rootCommand, string name, string fullName)
        {
            return new HelpPrinter
            {
                rootCommand = rootCommand,
                name = name,
                fullName = fullName,
            };
        }

        public HelpPrinter AddUsage(string usage)
        {
            usages.Add(usage);
            return this;
        }

        public HelpPrinter AddExample(string example)
        {
            examples.Add(example);
            return this;
        }

        public HelpPrinter AddArgument(string argument, string description)
        {
            arguments.Add($"{argument.PadRight(descColumn)}{description}");
            return this;
        }

        public HelpPrinter AddNote(string note, string description)
        {
            notes.Add($"{note.PadRight(descColumn)}{description}");
            return this;
        }

        public HelpPrinter StartCustomSection(string section)
        {
            if (!section.EndsWith(":"))
                section += ":";
            nonStandartSections.Add(section, new List<string>());
            lastSection = section;
            return this;
        }

        public HelpPrinter AddItem(string option, string description)
        {
            if (string.IsNullOrEmpty(lastSection))
                throw new InvalidOperationException("Call StartSection() before AddLine().");
            nonStandartSections[lastSection].Add($"{option.PadRight(descColumn)}{description}");
            return this;
        }

        public HelpPrinter SetDescription(string description)
        {
            this.description = description;
            return this;
        }

        public HelpPrinter SetIndent(int indent)
        {
            this.indent = indent;
            return this;
        }

        public HelpPrinter SetDescriptionColumn(int descriptionColumn)
        {
            descColumn = descriptionColumn;
            return this;
        }

        public void Print()
        {
            if (rootCommand.Count == 0)
                Indent(0, $"{fullName} ({name})");
            else
            {
                Indent(0, $"{fullName} ({string.Join(" ", rootCommand)} {name})");
            }
            PrintDescriptionSection();
            PrintUsageSection();
            PrintArgumentsSection();
            PrintNoteSection();
            PrintExamplesSection();
            PrintNonStandartSections();
        }

        private void PrintNonStandartSections()
        {
            if (nonStandartSections.Count > 0)
            {
                foreach (var section in nonStandartSections)
                {
                    Indent(1, section.Key);
                    foreach (string line in section.Value)
                        Indent(2, line);
                }
            }
        }

        private void PrintExamplesSection()
        {
            if (examples.Count > 0)
            {
                Indent(1, "Examples:");
                foreach (string example in examples)
                {
                    if (rootCommand.Count == 0)
                        Indent(2, $"{name} {example}");
                    else
                    {
                        Indent(2, $"{string.Join(" ", rootCommand)} {name} {example}");
                    }
                }
            }
        }

        private void PrintNoteSection()
        {
            if (notes.Count > 0)
            {
                Indent(1, "Notes:");
                foreach (string note in notes)
                {
                    Indent(2, note);
                }
            }
        }

        private void PrintArgumentsSection()
        {
            if (arguments.Count > 0)
            {
                Indent(1, "Arguments:");
                foreach (string argument in arguments)
                {
                    Indent(2, argument);
                }
            }
        }

        private void PrintUsageSection()
        {
            if (usages.Count > 0)
            {
                Indent(1, "Usage:");
                foreach (string usage in usages)
                {
                    if (rootCommand.Count == 0)
                    {
                        Indent(2, $"{name} {usage}");
                    }
                    else
                    {
                        Indent(2, $"{string.Join(" ", rootCommand)} {name} {usage}");
                    }
                }
            }
        }

        private void PrintDescriptionSection()
        {
            if (!string.IsNullOrEmpty(description))
            {
                Indent(1, $"Description:");
                Indent(2, description);
            }
        }

        private void Indent(int indent, string text = "")
        {
            Console.WriteLine($"{new string(' ', this.indent * indent)}{text}");
        }
    }
}
