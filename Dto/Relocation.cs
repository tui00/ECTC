using System.Collections.Generic;

namespace Ectc.Dto
{
    public class Relocation
    {
        public string Name { get; set; }
        public List<ushort> Usings { get; set; }

        public Relocation(string name, List<ushort> usings)
        {
            Name = name;
            Usings = usings;
        }

        public Relocation(string name)
        {
            Name = name;
            Usings = new List<ushort>();
        }

        public Relocation()
        {
            Name = "";
            Usings = new List<ushort>();
        }

        public void AddUsing(ushort usingAddress)
        {
            Usings.Add(usingAddress);
        }
    }
}
