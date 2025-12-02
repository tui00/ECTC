using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ectc.Dto
{
    public class ObjectFile
    {
        public List<Section> Sections { get; set; }
        public List<Symbol> Symbols { get; set; }
        public List<Relocation> Relocations { get; set; }

        public static byte[] Magic => Encoding.ASCII.GetBytes("EctcObj");

        public ObjectFile(List<Section> sections, List<Symbol> symbols, List<Relocation> relocations)
        {
            Sections = sections;
            Symbols = symbols;
            Relocations = relocations;
            TestValid();
        }

        public ObjectFile()
        {
            Sections = new List<Section>();
            Symbols = new List<Symbol>();
            Relocations = new List<Relocation>();
        }

        public void TestValid()
        {
            if (Sections.Count == 0)
            {
                throw new InvalidProgramException("No sections");
            }
            foreach (var section in Sections)
            {
                if (section.Data.Length == 0)
                {
                    if (section.IsRelocatable)
                    {
                        throw new InvalidProgramException($"Relocatable section has no data");
                    }
                    else
                    {
                        throw new InvalidProgramException($"Section with address '{section.Address}' has no data");
                    }
                }
            }
        }

        public byte[] ToBytes()
        {
            var b = new List<byte>();
            b.AddRange(Magic);

            Add16((ushort)(Sections.Count - 1));
            foreach (var section in Sections)
            {
                b.Add((byte)(section.IsRelocatable ? 1 : 0));
                if (!section.IsRelocatable)
                    Add16(section.Address);
                Add16((ushort)(section.Data.Length - 1));
                AddRange16(section.Data);
            }
            Add16((ushort)Symbols.Count);
            foreach (var symbol in Symbols)
            {
                foreach (var c in symbol.Name)
                {
                    b.Add((byte)c);
                }
                b.Add(0);
                Add16(symbol.Address);
            }
            Add16((ushort)Relocations.Count);
            foreach (var symbol in Relocations)
            {
                foreach (var c in symbol.Name)
                {
                    b.Add((byte)c);
                }
                b.Add(0);
                Add16((ushort)symbol.Usings.Count);
                AddRange16(symbol.Usings);
            }
            return b.ToArray();

            void Add16(ushort value)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                b.AddRange(bytes);
            }

            void AddRange16(IEnumerable<ushort> values)
            {
                foreach (var value in values)
                {
                    Add16(value);
                }
            }
        }

        public static ObjectFile FromBytes(byte[] data)
        {
            int off = 0;
            var obj = new ObjectFile();
            if (!data.Take(Magic.Length).SequenceEqual(Magic))
                throw new InvalidProgramException("Invalid object file format");
            off += Magic.Length;
            int sectionsCount = Read16() + 1;
            for (int i = 0; i < sectionsCount; i++)
            {
                var section = new Section
                {
                    IsRelocatable = data[off++] == 1
                };
                if (!section.IsRelocatable)
                    section.Address = Read16();
                int dataLen = Read16() + 1;
                section.Data = new ushort[dataLen];
                for (int j = 0; j < dataLen; j++)
                    section.Data[j] = Read16();
                obj.Sections.Add(section);
            }
            int symbolsCount = Read16();
            for (int i = 0; i < symbolsCount; i++)
            {
                var sym = new Symbol
                {
                    Name = ReadCString(),
                    Address = Read16()
                };
                obj.Symbols.Add(sym);
            }
            int relocationsCount = Read16();
            for (int i = 0; i < relocationsCount; i++)
            {
                var rel = new Relocation
                {
                    Name = ReadCString()
                };
                int usingCount = Read16();
                rel.Usings = new List<ushort>(usingCount);
                for (int j = 0; j < usingCount; j++)
                    rel.Usings.Add(Read16());
                obj.Relocations.Add(rel);
            }
            return obj;

            ushort Read16()
            {
                var v = BitConverter.ToUInt16(data, off);
                off += 2;
                return v;
            }

            string ReadCString()
            {
                var sb = new List<byte>();
                while (data[off] != 0)
                    sb.Add(data[off++]);
                off++; // skip null
                return System.Text.Encoding.ASCII.GetString(sb.ToArray());
            }
        }
    }
}
