namespace Ectc.Dto
{
    public class Section
    {
        public UInt12 Address { get; }
        public UInt12[] Data { get; }
        public bool IsRelocatable { get; }

        public Section(UInt12[] data, UInt12 address, bool isRelocatable = true)
        {
            Address = address;
            Data = data;
            IsRelocatable = isRelocatable;
        }
    }
}
