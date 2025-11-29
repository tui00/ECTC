using System;
using System.Linq;

namespace Ectc.InstructionTable
{
    public class Instruction
    {
        public InstructionInfo Info { get; }
        public InstructionArgument[] Arguments { get; }

        public Instruction(InstructionInfo info, InstructionArgument[] arguments)
        {
            Info = info;
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
