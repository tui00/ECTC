using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ectc.Dto;

namespace Ectc.Linker
{
    public static class EctcLinker
    {
        public static byte[] Link(ObjectFile[] files)
        {
            FirstPassResult firstPass = FirstPass(files);
            var secondPass = SecondPass(firstPass);
            return secondPass;
        }

        private sealed class FirstPassResult
        {
            public List<Symbol> Symbols { get; set; }
            public List<Relocation> Relocations { get; set; }
            public List<Section> Sections { get; set; }

            public FirstPassResult(List<Symbol> symbols, List<Relocation> relocations, List<Section> sections)
            {
                Symbols = symbols;
                Relocations = relocations;
                Sections = sections;
            }
        }

        private static FirstPassResult FirstPass(ObjectFile[] files)
        {
            var symbols = new List<Symbol>();
            var relocations = new List<Relocation>();
            var symbolsSet = new HashSet<string>();
            var sections = new List<Section>();
            var sectionsSet = new HashSet<ushort>();

            foreach (var file in files)
            {
                foreach (var relocation in file.Relocations)
                {
                    relocations.Add(relocation);
                }

                foreach (var symbol in file.Symbols)
                {
                    if (!symbolsSet.Add(symbol.Name))
                        throw new DuplicateNameException($"Duplicate symbol name: '{symbol.Name}'");
                    symbols.Add(symbol);
                }

                foreach (var section in file.Sections)
                {
                    if (!sectionsSet.Add(section.Address))
                        throw new DuplicateNameException($"Duplicate section address: '{section.Address}'");
                    sections.Add(section);
                }
            }

            var unresolvedSymbols = relocations.Select(x => x.Name).Except(symbolsSet);
            if (unresolvedSymbols.Any())
                throw new InvalidProgramException($"Unresolved symbols: {string.Join(", ", unresolvedSymbols)}");
            return new FirstPassResult(symbols, relocations, sections);
        }

        private static byte[] SecondPass(FirstPassResult firstPass)
        {
            // TODO: Просто объеденить секции заменив значения по Usings в релокациях на Address из Symbols[rel.Name]
            throw new NotImplementedException();
        }
    }
}