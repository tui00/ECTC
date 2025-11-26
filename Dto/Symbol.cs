namespace Ectc.Dto
{
    public class Symbol
    {
        public string Name { get; }
        public UInt12 Address { get; }
        public UInt12[] References { get; set; }
        public bool IsExternal { get; set; }

        public Symbol(string name, UInt12 address, UInt12[] references)
        {
            Name = name;
            Address = address;
            References = references;
            IsExternal = false;
        }

        public Symbol(string name, UInt12[] references)
        {
            Name = name;
            Address = default;
            References = references;
            IsExternal = true;
        }
    }
}
