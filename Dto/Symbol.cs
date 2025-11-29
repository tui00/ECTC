namespace Ectc.Dto
{
    public class Symbol
    {
        public string Name { get; }
        public UInt12 Address { get; }

        public Symbol(string name, UInt12 address)
        {
            Name = name;
            Address = address;
        }
    }
}
