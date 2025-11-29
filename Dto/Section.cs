namespace Ectc.Dto
{
    public class Section
    {
        public ushort Address { get; }
        public ushort[] Data { get; }
        public bool IsRelocatable { get; }

        public Section(ushort[] data, ushort address, bool isRelocatable = true)
        {
            Address = address;
            Data = data;
            IsRelocatable = isRelocatable;
        }
    }
}
