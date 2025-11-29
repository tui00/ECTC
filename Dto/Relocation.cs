using System.Collections.Generic;

namespace Ectc.Dto
{
    public class Relocation
    {
        public string Name { get; }
        public List<UInt12> Usings { get; }

        public Relocation(string name, List<UInt12> usings)
        {
            Name = name;
            Usings = usings;
        }

        public Relocation(string name)
        {
            Name = name;
            Usings = new List<UInt12>();
        }

        public void AddUsing(UInt12 usingAddress)
        {
            Usings.Add(usingAddress);
        }
    }
}
