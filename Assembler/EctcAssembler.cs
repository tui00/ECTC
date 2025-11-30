using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ectc.Dto;
using Ectc.InstructionTable;

namespace Ectc.Assembler
{
    public static class EctcAssembler
    {
        private sealed class AsmLine
        {
            public string Label { get; }
            public string Operation { get; }
            public string[] Arguments { get; }
            public int LineNumber { get; }

            public AsmLine(string label, string operation, string[] arguments, int lineNumber)
            {
                Label = label;
                Operation = operation;
                Arguments = arguments;
                LineNumber = lineNumber;
            }

            public static AsmLine Parse(string line, int lineNumber)
            {
                if (line == null)
                    throw new ArgumentNullException(nameof(line), "Line is null");
                line = line.Trim();
                if (line.Length == 0)
                {
                    return new AsmLine(null, null, null, lineNumber);
                }

                if (line.Contains(";"))
                    line = line.Substring(line.IndexOf(";"));
                line = line.Trim();

                string label = null;
                if (line.Contains(":"))
                {
                    label = line.Substring(0, line.IndexOf(":"));
                    line = line.Substring(line.IndexOf(":") + 1).Trim();
                    TestLabel(label);
                }

                string operation = null;
                string[] arguments = null;
                if (!string.IsNullOrEmpty(line))
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

                return new AsmLine(label, operation, arguments, lineNumber);
            }

            private static void TestLabel(string label)
            {
                if (string.IsNullOrEmpty(label))
                    throw new ArgumentException("Label is null or empty", nameof(label));

                if (label.Contains(" "))
                {
                    throw new ArgumentException("Label cannot contain spaces", nameof(label));
                }
                if (label.Contains(":"))
                {
                    throw new ArgumentException("Label cannot contain ':'", nameof(label));
                }

                bool charsCorrect = label.All(c => char.IsLetterOrDigit(c) || c == '_');
                if (char.IsDigit(label[0]))
                {
                    throw new ArgumentException("Label cannot start with a digit", nameof(label));
                }
                if (!charsCorrect)
                {
                    throw new ArgumentException("Label contains invalid characters", nameof(label));
                }
            }
        }

        public static ObjectFile Assemble(string sourceCode)
        {
            var code = new List<AsmLine>();
            string[] formatedCode = sourceCode.Replace("\r", "").Split('\n');
            for (int i = 0; i < formatedCode.Length; i++)
            {
                string line = formatedCode[i];
                try
                {
                    code.Add(AsmLine.Parse(line, code.Count));
                }
                catch (Exception e)
                {
                    e.Data["LineNumber"] = i;
                    throw;
                }
            }
            ObjectFile result = SecondPass(code.ToArray(), FirstPass(code.ToArray()));
            return result;
        }

        private static ObjectFile FirstPass(AsmLine[] code)
        {
            var symbolsSet = new HashSet<string>();
            var symbols = new List<Symbol>();

            var relocationsSet = new HashSet<string>();
            var relocations = new Dictionary<string, Relocation>();

            ushort nextAddress = 0;
            for (int i = 0; i < code.Length; i++)
            {
                ProcesLine(code[i], i);
            }

            return new ObjectFile(new List<Section>(), symbols, relocations.Values.ToList());

            void AddSymbol(string label)
            {
                if (!symbolsSet.Add(label))
                    throw new DuplicateNameException($"Symbol {label} already exists");
                symbols.Add(new Symbol(label, nextAddress));
            }

            void AddRelocations(string[] arguments)
            {
                foreach (var argument in arguments.Where(relocationsSet.Add))
                {
                    relocations.Add(argument, new Relocation(argument));
                }

                for (int i = 0; i < arguments.Length; i++)
                {
                    string argument = arguments[i];
                    relocations[argument].AddUsing((ushort)(nextAddress + 1 + i));
                }
            }

            void ProcesInstruction(Instruction instruction)
            {
                if (instruction.Arguments != null)
                {
                    var labels = instruction.Arguments?
                        .Where(arg => arg.Type == InstructionArgumentType.LabelOrImmediate)
                        .Where(arg => !TryParse(arg.Value, out _))
                        .Select(arg => arg.Value)
                        .ToArray();
                    AddRelocations(labels);
                }
                nextAddress += (ushort)instruction.Size;
            }

            void ProcesLine(AsmLine line, int lineNumber)
            {
                try
                {
                    if (line.Label != null)
                        AddSymbol(line.Label);

                    if (!line.Operation?.StartsWith(".") == true)
                        ProcesInstruction(Instruction.Parse(line.Operation, line.Arguments));
                }
                catch (Exception e)
                {
                    e.Data["LineNumber"] = lineNumber;
                    throw;
                }
            }
        }

        private static ObjectFile SecondPass(AsmLine[] code, ObjectFile firstPass)
        {
            var symbols = firstPass.Symbols;
            var relocations = firstPass.Relocations.ToDictionary(r => r.Name, r => r.Usings);

            var chunks = SplitByOrg(code);
            var sections = new Dictionary<ushort, Section>();
            var sectionsSet = new HashSet<ushort>();

            foreach (var chunk in chunks)
            {
                var section = AssemblyChunkToSection(chunk, relocations);
                if (!sectionsSet.Add(chunk.Org))
                    throw new DuplicateNameException($"Section with address '{chunk.Org}' already exists");

                sections.Add(chunk.Org, section);
            }

            return new ObjectFile(sections.Values.ToList(), symbols, firstPass.Relocations);
        }


        private sealed class SectionChunk
        {
            public ushort Org { get; }
            public List<AsmLine> Lines { get; }

            public SectionChunk(ushort org)
            {
                Org = org;
                Lines = new List<AsmLine>();
            }
        }

        private static List<SectionChunk> SplitByOrg(AsmLine[] code)
        {
            var result = new List<SectionChunk>();
            ushort currentOrg = 0;
            var currentChunk = new SectionChunk(currentOrg);

            foreach (var line in code)
            {
                if (!string.IsNullOrEmpty(line.Operation) && line.Operation == ".org")
                {
                    result.Add(currentChunk);
                    if (line.Arguments == null || line.Arguments.Length != 1)
                        throw new ArgumentException("Invalid number of arguments for '.org' directive");
                    if (!TryParse(line.Arguments[0], out currentOrg))
                        throw new ArgumentException("Invalid argument for '.org' directive");
                    currentChunk = new SectionChunk(currentOrg);
                    continue;
                }

                currentChunk.Lines.Add(line);
            }

            result.Add(currentChunk);
            return result;
        }

        private static Section AssemblyChunkToSection(SectionChunk chunk, Dictionary<string, List<ushort>> relocations)
        {
            var words = new Dictionary<ushort, ushort>();
            ushort nextAddress = 0;

            foreach (var line in chunk.Lines)
            {
                try
                {
                    if (string.IsNullOrEmpty(line.Operation)) continue;
                    ProcessInstruction(Instruction.Parse(line.Operation, line.Arguments));
                }
                catch (Exception e)
                {
                    e.Data["LineNumber"] = line.LineNumber;
                    throw;
                }
            }

            ushort maxIndex = words.Keys.Max();

            var sectionArray = new ushort[maxIndex + 1];
            foreach (var kv in words)
                sectionArray[kv.Key] = kv.Value;

            return new Section(sectionArray, chunk.Org);

            void ProcessInstruction(Instruction instruction)
            {
                if (instruction == null)
                    throw new ArgumentNullException(nameof(instruction));

                var labelArgs = instruction.Arguments?
                    .Where(arg => arg.Type == InstructionArgumentType.LabelOrImmediate)
                    .Select(arg => arg.Value)
                    .ToArray() ?? new string[] { };

                words[nextAddress] = instruction.Code;

                for (int argIndex = 0; argIndex < labelArgs.Length; argIndex++)
                {
                    string arg = labelArgs[argIndex];
                    var argAddr = (ushort)(nextAddress + 1 + argIndex);

                    // Если аргумент метка и его нет в списке релокаций
                    if (!TryParse(arg, out ushort value) && !(relocations.TryGetValue(arg, out var uses) && uses.Contains(argAddr)))
                        throw new ArgumentException($"Unknown label '{arg}'");

                    words[argAddr] = value;
                }

                nextAddress = (ushort)(nextAddress + instruction.Size);
            }
        }

        private static bool TryParse(string value, out ushort output)
        {
            try
            {
                value = value.Trim();
                if (value.StartsWith("0x"))
                {
                    output = Convert.ToUInt16(value.Substring(2), 16);
                    return true;
                }
                else if (value.StartsWith("0b"))
                {
                    output = Convert.ToUInt16(value.Substring(2), 2);
                    return true;
                }
                else if (value.StartsWith("0") && value.Length > 1)
                {
                    output = Convert.ToUInt16(value.Substring(2), 8);
                    return true;
                }
                else if (value.StartsWith("-") && short.TryParse(value, out short tmp))
                {
                    output = (ushort)tmp;
                    return true;
                }
                else
                {
                    return ushort.TryParse(value, out output);
                }
            }
            catch
            {
                output = 0;
                return false;
            }
        }
    }
}
