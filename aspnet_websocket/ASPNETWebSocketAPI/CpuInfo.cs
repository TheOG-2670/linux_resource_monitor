namespace LinuxResourceMonitorApi
{
    public class CpuInfo
    {
        public CpuInfo() { }
        public CpuInfo(int freq) 
        {
            Frequency = freq;
        }
        public long Frequency { get; set; }
    }
}
