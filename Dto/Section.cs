namespace Ectc.Dto
{
    public class Section
    {
        public UInt12 Address { get; }
        public byte[] Data { get; }
        public bool IsRelocatable { get; }

        public Section(byte[] data, UInt12 address, bool isRelocatable = true)
        {
            Address = address;
            Data = data;
            IsRelocatable = isRelocatable;
        }
    }
}
