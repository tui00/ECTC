using System;
using System.Collections.Generic;
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

            private AsmLine(string label, string operation, string[] args, int lineNumber)
            {
                Label = label;
                Operation = operation;
                Arguments = args;
                LineNumber = lineNumber;
            }

            public static AsmLine Parse(string line, int lineNumber)
            {
                if (line == null) throw new ArgumentNullException(nameof(line));

                line = line.Split(new[] { ';' }, 2)[0].Trim();
                if (line.Length == 0)
                    return new AsmLine(null, null, null, lineNumber);

                string label = null;

                if (line.Contains(":"))
                {
                    var parts = line.Split(new[] { ':' }, 2);
                    label = parts[0].Trim();
                    ValidateLabel(label);
                    line = parts[1].Trim();
                }

                if (string.IsNullOrWhiteSpace(line))
                    return new AsmLine(label, null, null, lineNumber);

                var opParts = line.Split(new[] { ' ' }, 2);
                var op = opParts[0];
                var args = opParts.Length > 1
                    ? opParts[1].Split(',').Select(a => a.Trim()).ToArray()
                    : null;

                return new AsmLine(label, op, args, lineNumber);
            }

            private static void ValidateLabel(string label)
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
                if (char.IsDigit(label[0]))
                {
                    throw new ArgumentException("Label cannot start with a digit", nameof(label));
                }
                if (label.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
                {
                    throw new ArgumentException("Label contains invalid characters", nameof(label));
                }
            }
        }

        public static ObjectFile Assemble(string source)
        {
            var lines = source
                .Replace("\r", "")
                .Split('\n')
                .Select((l, i) =>
                {
                    try { return AsmLine.Parse(l, i); }
                    catch (Exception e)
                    {
                        e.Data["LineNumber"] = i;
                        throw;
                    }
                })
                .ToArray();

            var first = FirstPass(lines);
            return SecondPass(lines, first);
        }

        private static ObjectFile FirstPass(AsmLine[] code)
        {
            var file = new ObjectFile();
            var relocations = new Dictionary<string, Relocation>();

            foreach (var chunk in SplitByOrg(code))
                ProcessChunk(chunk, file, relocations);

            file.Relocations = relocations.Values.ToList();
            return file;
        }

        private static void ProcessChunk(
            SectionChunk chunk,
            ObjectFile file,
            Dictionary<string, Relocation> relocations)
        {
            ushort nextAddr = 0;

            foreach (var line in chunk.Lines)
            {
                try
                {
                    AddLabelIfExists(line, file, chunk.Org, nextAddr);
                    ProcessInstructionIfExists(line, chunk, relocations, ref nextAddr);
                }
                catch (Exception e)
                {
                    e.Data["LineNumber"] = line.LineNumber;
                    throw;
                }
            }
        }

        private static void AddLabelIfExists(
            AsmLine line,
            ObjectFile file,
            ushort org,
            ushort nextAddr)
        {
            if (line.Label == null) return;

            file.Symbols.Add(new Symbol(
                line.Label,
                (ushort)(org + nextAddr)
            ));
        }

        private static void ProcessInstructionIfExists(
            AsmLine line,
            SectionChunk chunk,
            Dictionary<string, Relocation> relocations,
            ref ushort nextAddr)
        {
            if (line.Operation == null || line.Operation.StartsWith("."))
                return;

            var instr = Instruction.Parse(line.Operation, line.Arguments);

            if (instr.Arguments != null)
                ProcessArguments(instr, chunk, relocations, nextAddr);

            nextAddr += (ushort)instr.Size;
        }

        private static void ProcessArguments(
            Instruction instruction,
            SectionChunk chunk,
            Dictionary<string, Relocation> relocations,
            ushort nextAddr)
        {
            var arguments = instruction.Arguments
                .Where(a => a.Type == InstructionArgumentType.LabelOrImmediate)
                .Select(a => a.Value)
                .ToArray();
            foreach (var argument in arguments)
            {
                var value = argument;

                if (TryParse(value, out _))
                    continue;

                if (!relocations.TryGetValue(value, out var rel))
                {
                    rel = new Relocation(value);
                    relocations[value] = rel;
                }

                rel.AddUsing((ushort)(chunk.Org + nextAddr + 1));
            }
        }

        private static ObjectFile SecondPass(AsmLine[] code, ObjectFile first)
        {
            var reloc = first.Relocations.ToDictionary(r => r.Name, r => r.Usings);
            var sections = SplitByOrg(code)
                .Select(c => AssemblyChunkToSection(c, reloc))
                .Where(s => s.Data.Length > 0)
                .ToList();

            return new ObjectFile(sections, first.Symbols, first.Relocations);
        }

        private sealed class SectionChunk
        {
            public ushort Org { get; }
            public List<AsmLine> Lines { get; }
            public bool IsRelocateble { get; }

            public SectionChunk(ushort org, bool isRelocatable)
            {
                Org = org;
                Lines = new List<AsmLine>();
                IsRelocateble = isRelocatable;
            }
        }

        private static List<SectionChunk> SplitByOrg(IEnumerable<AsmLine> code)
        {
            var result = new List<SectionChunk>();
            var chunk = new SectionChunk(0, true);

            foreach (var line in code)
            {
                if (line.Operation == ".org")
                {
                    result.Add(chunk);

                    if (line.Arguments?.Length != 1)
                        throw new ArgumentException("Invalid number of arguments for '.org' directive");
                    if (!TryParse(line.Arguments[0], out ushort org))
                        throw new ArgumentException("Invalid argument for '.org' directive");

                    chunk = new SectionChunk(org, false);
                }
                else
                {
                    chunk.Lines.Add(line);
                }
            }

            result.Add(chunk);
            return result;
        }


        private static Section AssemblyChunkToSection(
            SectionChunk chunk,
            Dictionary<string, List<ushort>> relocations)
        {
            var words = new Dictionary<ushort, ushort>();
            ushort addr = 0;

            foreach (var line in chunk.Lines)
            {
                if (string.IsNullOrWhiteSpace(line.Operation)) continue;

                var instr = Instruction.Parse(line.Operation, line.Arguments);

                words[addr] = instr.Code;

                var labelArgs = instr.Arguments?
                    .Where(a => a.Type == InstructionArgumentType.LabelOrImmediate)
                    .Select(a => a.Value)
                    .ToArray() ?? new string[] { };

                for (int i = 0; i < labelArgs.Length; i++)
                {
                    var argAddr = (ushort)(addr + 1 + i);
                    var arg = labelArgs[i];

                    if (!TryParse(arg, out var val) &&
                        !(relocations.TryGetValue(arg, out var uses) && uses.Contains(argAddr)))
                        throw new ArgumentException($"Unknown label '{arg}'");

                    words[argAddr] = val;
                }

                addr += (ushort)instr.Size;
            }

            if (words.Count == 0)
                return new Section(new ushort[] { }, chunk.Org, chunk.IsRelocateble);

            var max = words.Keys.Max();
            var data = new ushort[max + 1];
            foreach (var w in words)
                data[w.Key] = w.Value;

            return new Section(data, chunk.Org, chunk.IsRelocateble);
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
