namespace Ectc.Dto
{
    public class Symbol
    {
        public string Name { get; }
        public ushort Address { get; }

        public Symbol(string name, ushort address)
        {
            Name = name;
            Address = address;
        }
    }
}
