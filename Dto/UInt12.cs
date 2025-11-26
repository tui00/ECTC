using System;

namespace Ectc.Dto
{
    public readonly struct UInt12 : IComparable, IComparable<UInt12>, IFormattable
    {
        public static readonly UInt12 MaxValue = new UInt12((1 << 12) - 1);
        public static readonly UInt12 MinValue = new UInt12(0);

        private readonly ushort _value;

        public UInt12(ushort value)
        {
            if (value >= (1 << 12))
                throw new OverflowException("Value must be 12 bits or less.");
            _value = value;
        }

        public UInt12(int value) : this((ushort)value) { }

        public static UInt12 Parse(string value)
        {
            if (value == "0") return new UInt12(0);
            if (value.StartsWith("0x")) return new UInt12(Convert.ToUInt16(value.Substring(2), 16));
            if (value.StartsWith("0b")) return new UInt12(Convert.ToUInt16(value.Substring(2), 2));
            if (value.StartsWith("0")) return new UInt12(Convert.ToUInt16(value.Substring(1), 8));
            if (value.StartsWith("-")) return new UInt12((ushort)short.Parse(value));
            if (value.StartsWith("+")) return new UInt12(ushort.Parse(value.Substring(1)));
            return new UInt12(ushort.Parse(value));
        }

        public static bool TryParse(string value, out UInt12 result)
        {
            try
            {
                result = Parse(value);
                return true;
            }
            catch
            {
                result = new UInt12(0);
                return false;
            }
        }

        public override int GetHashCode() => _value.GetHashCode();
        public override bool Equals(object obj) => obj is UInt12 other && _value == other._value;
        public int CompareTo(object obj) => _value.CompareTo(obj);
        public int CompareTo(UInt12 other) => _value.CompareTo(other._value);
        public override string ToString() => _value.ToString();
        public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);

        public static UInt12 operator +(UInt12 a, UInt12 b) => new UInt12(a._value + b._value);
        public static UInt12 operator -(UInt12 a, UInt12 b) => new UInt12(a._value - b._value);
        public static UInt12 operator *(UInt12 a, UInt12 b) => new UInt12(a._value * b._value);
        public static UInt12 operator /(UInt12 a, UInt12 b) => new UInt12(a._value / b._value);
        public static UInt12 operator %(UInt12 a, UInt12 b) => new UInt12(a._value % b._value);
        public static UInt12 operator &(UInt12 a, UInt12 b) => new UInt12(a._value & b._value);
        public static UInt12 operator |(UInt12 a, UInt12 b) => new UInt12(a._value | b._value);
        public static UInt12 operator ^(UInt12 a, UInt12 b) => new UInt12(a._value ^ b._value);
        public static UInt12 operator ~(UInt12 a) => new UInt12(~a._value);
        public static UInt12 operator +(UInt12 a) => a;
        public static UInt12 operator -(UInt12 a) => new UInt12(-a._value);
        public static UInt12 operator ++(UInt12 a) => new UInt12(a._value + 1);
        public static UInt12 operator --(UInt12 a) => new UInt12(a._value - 1);
        public static bool operator ==(UInt12 a, UInt12 b) => a._value == b._value;
        public static bool operator !=(UInt12 a, UInt12 b) => a._value != b._value;
        public static bool operator <(UInt12 a, UInt12 b) => a._value < b._value;
        public static bool operator >(UInt12 a, UInt12 b) => a._value > b._value;
        public static bool operator <=(UInt12 a, UInt12 b) => a._value <= b._value;
        public static bool operator >=(UInt12 a, UInt12 b) => a._value >= b._value;
        public static implicit operator UInt12(byte b) => new UInt12(b);
        public static implicit operator UInt12(sbyte b) => new UInt12((ushort)b);
        public static implicit operator UInt12(short i) => new UInt12((ushort)i);
        public static implicit operator UInt12(ushort i) => new UInt12(i);
        public static implicit operator UInt12(int i) => new UInt12((ushort)i);
        public static implicit operator UInt12(uint i) => new UInt12((ushort)i);
        public static implicit operator UInt12(long i) => new UInt12((ushort)i);
        public static implicit operator UInt12(ulong i) => new UInt12((ushort)i);
        public static implicit operator UInt12(float i) => new UInt12((ushort)i);
        public static implicit operator UInt12(double i) => new UInt12((ushort)i);
        public static implicit operator UInt12(decimal i) => new UInt12((ushort)i);
        public static implicit operator UInt12(char i) => new UInt12(i);
        public static explicit operator byte(UInt12 i) => (byte)i._value;
        public static explicit operator sbyte(UInt12 i) => (sbyte)i._value;
        public static explicit operator short(UInt12 i) => (short)i._value;
        public static explicit operator ushort(UInt12 i) => i._value;
        public static explicit operator int(UInt12 i) => i._value;
        public static explicit operator uint(UInt12 i) => i._value;
        public static explicit operator long(UInt12 i) => i._value;
        public static explicit operator ulong(UInt12 i) => i._value;
        public static explicit operator float(UInt12 i) => i._value;
        public static explicit operator double(UInt12 i) => i._value;
        public static explicit operator decimal(UInt12 i) => i._value;
        public static explicit operator char(UInt12 i) => (char)i._value;
    }

}
