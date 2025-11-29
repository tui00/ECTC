using System.Collections.Generic;

namespace Ectc.Dto
{
    public class ObjectFile
    {
        public List<Section> Sections { get; }
        public List<Symbol> DefinedSymbols { get; }
        public List<Relocation> Relocations { get; }

        public ObjectFile(List<Section> sections, List<Symbol> definedSymbols, List<Relocation> externalSymbols)
        {
            Sections = sections;
            DefinedSymbols = definedSymbols;
            Relocations = externalSymbols;
        }

        public UInt12[] ToWords()
        {
            var b = new List<UInt12>();

            b.Add(Sections.Count);
            foreach (var section in Sections)
            {
                b.Add(section.IsRelocatable ? 1 : 0);
                if (section.IsRelocatable)
                    b.Add(section.Address);
                b.Add(section.Data.Length);
                b.AddRange(section.Data);
            }
            b.Add(DefinedSymbols.Count);
            foreach (var symbol in DefinedSymbols)
            {
                foreach (var c in symbol.Name)
                {
                    b.Add(c);
                }
                b.Add(0);
                b.Add(symbol.Address);
            }
            b.Add(Relocations.Count);
            foreach (var symbol in Relocations)
            {
                foreach (var c in symbol.Name)
                {
                    b.Add(c);
                }
                b.Add(0);
                b.Add(symbol.Usings.Count);
                b.AddRange(symbol.Usings);
            }
            return b.ToArray();
        }
    }
}
