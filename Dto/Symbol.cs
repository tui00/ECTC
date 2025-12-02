namespace Ectc.Dto
{
    public class Symbol
    {
        public string Name { get; set; }
        public ushort Address { get; set; }

        public Symbol(string name, ushort address)
        {
            Name = name;
            Address = address;
        }

        public Symbol()
        {
            Name = "";
            Address = 0;
        }
    }
}
