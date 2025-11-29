using Ectc.Dto;

namespace Ectc.InstructionTable
{
    public class InstructionInfo
    {
        public string Mnemonic { get; }
        public UInt12 Code { get; }
        public InstructionArgumentType[] Arguments { get; }
        public int Size => Arguments.Length + 1;

        public InstructionInfo(string mnemonic, UInt12 opcode, InstructionArgumentType[] arguments)
        {
            Mnemonic = mnemonic;
            Code = opcode;
            Arguments = arguments;
        }
    }
}