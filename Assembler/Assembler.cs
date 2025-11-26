using Ectc.Dto;

namespace Ectc.Assembler
{
    public static class Assembler
    {
        public static ObjectFile Assemble(string sourceCode)
        {
            string[] code = sourceCode.Replace("\r", "").Split('\n');
            ObjectFile result = SecondPass(code, FirstPass(code));
            return result;
        }

        private static ObjectFile FirstPass(string[] code)
        {
            return default;
        }

        private static ObjectFile SecondPass(string[] code, ObjectFile firstPass)
        {
            return firstPass;
        }

        private static void ParseLine(string line, out string label, out string operation, out string[] arguments)
        {
            label = default;
            operation = default;
            arguments = default;
        }
    }
}