using System;
using System.Linq;

namespace Ectc.InstructionTable
{
    public class Instruction
    {
        public InstructionArgument[] Arguments { get; }
        public string Mnemonic => info.Mnemonic;
        public ushort Code
        {
            get
            {
                byte registers = 0;
                InstructionArgument[] registerArmguments = Arguments.Where(arg => arg.Type == InstructionArgumentType.Register).ToArray();
                for (int i = 0; i < registerArmguments.Length; i++)
                {
                    byte regNum = 0;
                    string regName = registerArmguments[i].Value.ToUpper();
                    if (regName == "A") regNum = 0;
                    else if (regName == "B") regNum = 1;
                    else if (regName == "C") regNum = 2;
                    else if (regName == "D") regNum = 3;
                    else if (regName == "PC") regNum = 4;
                    else if (regName == "SP") regNum = 5;
                    else if (regName == "FLAGS") regNum = 6;
                    registers |= (byte)(regNum << (i * 4));
                }
                return (ushort)(info.BaseCode << 8 | registers);
            }
        }

        public int Size => info.Size;

        private readonly InstructionInfo info;

        public Instruction(InstructionInfo info, InstructionArgument[] arguments)
        {
            this.info = info;
            Arguments = arguments;
        }

        public static Instruction Parse(string mnemonic, string[] arguments)
        {
            var candidates = InstructionTable.CodeTable.Values
                .Where(info => info.Mnemonic.Equals(mnemonic.ToUpper(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!candidates.Any())
            {
                throw new ArgumentException($"Command '{mnemonic}' not found");
            }

            if (arguments == null)
            {
                if (candidates.Any(info => info.Arguments.Length == 0))
                {
                    return new Instruction(candidates.First(info => info.Arguments.Length == 0), new InstructionArgument[0]);
                }
                else
                {
                    throw new ArgumentException($"Command '{mnemonic}' requires arguments");
                }
            }

            var parsedArgs = arguments.Select(InstructionArgument.Parse).ToArray();
            var argTypes = parsedArgs.Select(arg => arg.Type).ToArray();

            InstructionInfo matchedInfo = candidates
                .FirstOrDefault(c => c.Arguments.Length == argTypes.Length && c.Arguments.SequenceEqual(argTypes));

            if (matchedInfo == null)
            {
                string formattedArguments = string.Join(", ", argTypes.Select(t =>
                    t == InstructionArgumentType.Register ? "<Register>" : "<Value>"));
                throw new ArgumentException($"Command '{mnemonic}' with arguments '{formattedArguments}' not found");
            }


            return new Instruction(matchedInfo, parsedArgs);
        }
    }
}
