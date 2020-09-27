namespace ProcessInfo.Models
{
#pragma warning disable 1591
    public class SystemStatus
    {
        public MemoryInformation Memory { get; set; }
        public ProcessInformation Process { get; set; }
        public string MachineName { get; set; }
        public string OSInfo { get; set; }
    }

    public class MemoryInformation
    {
        public string Total { get; set; }
        public string Used { get; set; }
        public string Free { get; set; }
    }

    public class ProcessInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string StartTime { get; set; }
        public string MemoryUsed { get; set; }
    }

        public static class MemMeasureType
    {
        public const string BIT = "bit";
        public const string BYTE = "Byte";
        public const string KBIT = "Kbit";
        public const string KBYTE = "KByte";
        public const string MEGABIT = "Mbit";
        public const string MEGABYTE = "MByte";
        public const string GIGABYTE = "GByte";
    }

    public class MemoryMetrics
    {
        public long Total { get; set; }
        public long Used { get; set; }

        public long Free { get; set; }
    }
}