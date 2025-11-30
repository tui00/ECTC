namespace Ectc.InstructionTable
{
    public class InstructionInfo
    {
        public string Mnemonic { get; }
        public ushort BaseCode { get; }
        public InstructionArgumentType[] Arguments { get; }
        public int Size { get; }

        public InstructionInfo(string mnemonic, int size, byte baseCode, InstructionArgumentType[] arguments)
        {
            Mnemonic = mnemonic;
            Size = size;
            BaseCode = baseCode;
            Arguments = arguments;
        }
    }
}
