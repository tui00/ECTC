using System;
using System.Linq;
using Ectc.Dto;

namespace Ectc.Assembler
{
    public static class EctcAssembler
    {
        private sealed class AsmLine
        {
            public string Label { get; }
            public string Operation { get; }
            public string[] Arguments { get; }

            public AsmLine(string label, string operation, string[] arguments)
            {
                Label = label;
                Operation = operation;
                Arguments = arguments;
            }

            public static AsmLine Parse(string line)
            {
                if (string.IsNullOrWhiteSpace(line))
                    throw new ArgumentNullException(nameof(line), "Line is null or empty");

                if (line.Contains(";"))
                    line = line.Substring(line.IndexOf(";"));
                line = line.Trim();

                string label = null;
                if (line.Contains(":"))
                {
                    label = line.Substring(0, line.IndexOf(":"));
                    line = line.Substring(line.IndexOf(":") + 1).Trim();
                }

                string operation = null;
                string[] arguments = null;
                if (!string.IsNullOrWhiteSpace(line))
                {
                    string[] parts = line.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    operation = parts[0].Trim();
                    if (parts.Length > 1)
                    {
                        arguments = parts[1].Split(',');
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            arguments[i] = arguments[i].Trim();
                        }
                    }
                }

                return new AsmLine(label, operation, arguments);
            }
        }

        public static ObjectFile Assemble(string sourceCode)
        {
            AsmLine[] code = sourceCode.Replace("\r", "").Split('\n').Select(AsmLine.Parse).ToArray();
            ObjectFile result = SecondPass(code, FirstPass(code));
            return result;
        }

        private static ObjectFile FirstPass(AsmLine[] code)
        {
            return default;
        }

        private static ObjectFile SecondPass(AsmLine[] code, ObjectFile firstPass)
        {
            return firstPass;
        }
    }
}