using System;

namespace Ectc.Assembler
{
    [Serializable]
    public class AssemblerFirstPassException : Exception
    {
        public AssemblerFirstPassException() { }
        public AssemblerFirstPassException(string message) : base(message) { }
        public AssemblerFirstPassException(string message, Exception inner) : base(message, inner) { }
        protected AssemblerFirstPassException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
