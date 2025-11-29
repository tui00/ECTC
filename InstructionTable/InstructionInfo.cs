namespace Ectc.InstructionTable
{
    public class InstructionInfo
    {
        public string Mnemonic { get; }
        public ushort Code { get; }
        public InstructionArgumentType[] Arguments { get; }
        public int Size => Arguments.Length + 1;

        public InstructionInfo(string mnemonic, ushort opcode, InstructionArgumentType[] arguments)
        {
            Mnemonic = mnemonic;
            Code = opcode;
            Arguments = arguments;
        }
    }
}