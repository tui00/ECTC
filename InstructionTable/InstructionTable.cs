using System.Collections.Generic;
using System.Linq;
using Ectc.Dto;

namespace Ectc.InstructionTable
{
    public static class InstructionTable
    {
        private const InstructionArgumentType Register = InstructionArgumentType.Register;
        private const InstructionArgumentType LabelOrImmediate = InstructionArgumentType.LabelOrImmediate;
        private static readonly InstructionArgumentType[] NoArguments = new InstructionArgumentType[] { };
        private static readonly InstructionArgumentType[] OneRegister = new[] { Register };
        private static readonly InstructionArgumentType[] TwoRegisters = new[] { Register, Register };
        private static readonly InstructionArgumentType[] RegisterAndImmediate = new[] { Register, LabelOrImmediate };
        private static readonly InstructionArgumentType[] OneImmediate = new[] { LabelOrImmediate };

        private static readonly InstructionInfo[] table = new InstructionInfo[]
        {
            new InstructionInfo("NOP",   0x00, NoArguments         ),
            new InstructionInfo("HLT",   0x01, NoArguments         ),
            new InstructionInfo("AND",   0x02, TwoRegisters        ),
            new InstructionInfo("NOT",   0x03, OneRegister         ),
            new InstructionInfo("OR",    0x04, TwoRegisters        ),
            new InstructionInfo("XOR",   0x05, TwoRegisters        ),
            new InstructionInfo("SHL",   0x06, OneRegister         ),
            new InstructionInfo("SHR",   0x07, OneRegister         ),
            new InstructionInfo("ADD",   0x08, TwoRegisters        ),
            new InstructionInfo("SAR",   0x09, OneRegister         ),
            new InstructionInfo("INC",   0x0A, OneRegister         ),
            new InstructionInfo("DEC",   0x0B, OneRegister         ),
            new InstructionInfo("CMP",   0x0C, TwoRegisters        ),
            new InstructionInfo("CMP",   0x0D, RegisterAndImmediate),
            new InstructionInfo("MOV",   0x0E, TwoRegisters        ),
            new InstructionInfo("LDI",   0x0F, RegisterAndImmediate),
            new InstructionInfo("ST",    0x10, TwoRegisters        ),
            new InstructionInfo("ST",    0x11, RegisterAndImmediate),
            new InstructionInfo("LD",    0x12, TwoRegisters        ),
            new InstructionInfo("LD",    0x13, RegisterAndImmediate),
            new InstructionInfo("PUSH",  0x14, OneRegister         ),
            new InstructionInfo("POP",   0x15, OneRegister         ),
            new InstructionInfo("JMP",   0x16, OneRegister         ),
            new InstructionInfo("JMP",   0x17, OneImmediate        ),
            new InstructionInfo("JZ",    0x18, OneRegister         ),
            new InstructionInfo("JZ",    0x19, OneImmediate        ),
            new InstructionInfo("JC",    0x1A, OneRegister         ),
            new InstructionInfo("JC",    0x1B, OneImmediate        ),
            new InstructionInfo("JNZ",   0x1C, OneRegister         ),
            new InstructionInfo("JNZ",   0x1D, OneImmediate        ),
            new InstructionInfo("JNC",   0x1E, OneRegister         ),
            new InstructionInfo("JNC",   0x1F, OneImmediate        ),
            new InstructionInfo("PUSHA", 0x20, NoArguments         ),
            new InstructionInfo("POPA",  0x21, NoArguments         ),
            new InstructionInfo("CALL",  0x22, OneRegister         ),
            new InstructionInfo("CALL",  0x23, OneImmediate        ),
            new InstructionInfo("RET",   0x24, NoArguments         ),
            new InstructionInfo("XCNG",  0x25, TwoRegisters        ),
        };

        private static readonly Dictionary<ushort, InstructionInfo> codeTable = table.ToDictionary(x => x.Code, x => x);

        public static Dictionary<ushort, InstructionInfo> CodeTable => codeTable;
    }
}
