using System.Collections.Generic;

namespace Ectc.Dto
{
    public class ObjectFile
    {
        public List<Section> Sections { get; }
        public List<Symbol> DefinedSymbols { get; }
        public List<Symbol> ExternalSymbols { get; }
    }
}
