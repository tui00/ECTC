namespace Ectc.InstructionTable
{
    public class InstructionInfo
    {
        public string Mnemonic { get; }
        public ushort Code { get; }
        public InstructionArgumentType[] Arguments { get; }
        public int Size { get; }

        public InstructionInfo(string mnemonic, int size, ushort opcode, InstructionArgumentType[] arguments)
        {
            Mnemonic = mnemonic;
            Size = size;
            Code = opcode;
            Arguments = arguments;
        }
    }
}
