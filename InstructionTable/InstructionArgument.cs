using System;
using System.Linq;
using Ectc.Dto;

namespace Ectc.InstructionTable
{
    public class InstructionArgument
    {
        public InstructionArgumentType Type { get; }
        public string Value { get; }

        public InstructionArgument(InstructionArgumentType type, string value)
        {
            Type = type;
            Value = value;
        }

        public static InstructionArgument Parse(string value)
        {
            if ((new[] { "A", "B", "C", "D" }).Contains(value.ToUpper()))
            {
                return new InstructionArgument(InstructionArgumentType.Register, value);
            }
            return new InstructionArgument(InstructionArgumentType.LabelOrImmediate, value);
        }
    }
}