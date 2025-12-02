namespace Ectc.Dto
{
    public class Section
    {
        public ushort Address { get; set; }
        public ushort[] Data { get; set; }
        public bool IsRelocatable { get; set; }

        public Section(ushort[] data, ushort address, bool isRelocatable = true)
        {
            Address = address;
            Data = data;
            IsRelocatable = isRelocatable;
        }

        public Section()
        {
            Data = new ushort[0];
            IsRelocatable = true;
            Address = 0;
        }
    }
}
