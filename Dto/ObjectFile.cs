using System.Collections.Generic;

namespace Ectc.Dto
{
    public class ObjectFile
    {
        public List<Section> Sections { get; }
        public List<Symbol> Symbols { get; }
        public List<Relocation> Relocations { get; }

        public ObjectFile(List<Section> sections, List<Symbol> symbols, List<Relocation> relocations)
        {
            Sections = sections;
            Symbols = symbols;
            Relocations = relocations;
        }

        public ushort[] ToWords()
        {
            var b = new List<ushort>();

            b.Add((ushort)Sections.Count);
            foreach (var section in Sections)
            {
                b.Add((ushort)(section.IsRelocatable ? 1 : 0));
                if (section.IsRelocatable)
                    b.Add(section.Address);
                b.Add((ushort)section.Data.Length);
                b.AddRange(section.Data);
            }
            b.Add((ushort)Symbols.Count);
            foreach (var symbol in Symbols)
            {
                foreach (var c in symbol.Name)
                {
                    b.Add(c);
                }
                b.Add(0);
                b.Add(symbol.Address);
            }
            b.Add((ushort)Relocations.Count);
            foreach (var symbol in Relocations)
            {
                foreach (var c in symbol.Name)
                {
                    b.Add(c);
                }
                b.Add(0);
                b.Add((ushort)symbol.Usings.Count);
                b.AddRange(symbol.Usings);
            }
            return b.ToArray();
        }
    }
}
