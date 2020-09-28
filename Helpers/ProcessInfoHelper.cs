using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using ProcessInfo.Models;

namespace ProcessInfo.Helpers
{
#pragma warning disable 1591


    public class ProcessInfoHelper
    {
        public ProcessInfoHelper()
        {}

        public SystemStatus GetSystemInfo()
        {
            MemoryMetrics memMetrics;

            if(IsUnix())
            {
                memMetrics = GetUnixMetrics();
            }
            else 
            {
                memMetrics = GetWindowsMetrics();
            }
            
            Process pr = Process.GetCurrentProcess();

            SystemStatus sysStat = new SystemStatus
            {
                Memory = new MemoryInformation(),
                Process = new ProcessInformation
                {
                    Status = "Not Responding"
                }
            };

            if (pr != null)
            {
                if (!pr.HasExited)
                {
                    pr.Refresh();

                    sysStat = new SystemStatus
                    {
                        MachineName = Environment.MachineName,
                        OSInfo = Environment.OSVersion.ToString(),
                        Architecture = Environment.Is64BitOperatingSystem ? "64bit" : "32bit",
                        Memory = new MemoryInformation
                        {
                            Total = $"{ConvertTo(MemMeasureType.KBYTE, MemMeasureType.GIGABYTE, memMetrics.Total)} GB",
                            Used = $"{ConvertTo(MemMeasureType.KBYTE, MemMeasureType.GIGABYTE, memMetrics.Used)} GB",
                            Free = $"{ConvertTo(MemMeasureType.KBYTE, MemMeasureType.GIGABYTE, memMetrics.Free)} GB"
                        },
                        Process = new ProcessInformation
                        {
                            Id = pr.Id.ToString(),
                            Name = pr.ProcessName,
                            StartTime = pr.StartTime.ToString("yyyy/MM/dd HH:mm:ss"),
                            MemoryUsed = $"{ConvertTo(MemMeasureType.BYTE, MemMeasureType.MEGABYTE, pr.WorkingSet64)} MB",
                            Status = pr.Responding ? "Running" : "Not Responding"
                        }
                    };
                }
            }

            return sysStat;
        }

        private bool IsUnix()
        {
            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                        RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            
            return isUnix;
        }
        
        private MemoryMetrics GetUnixMetrics()
        {
            var output = "";
    
            var info = new ProcessStartInfo("free -m");
            info.FileName = "/bin/bash";
            info.Arguments = "-c \"free -k\"";
            info.RedirectStandardOutput = true;
            
            using(var process = Process.Start(info))
            {                
                output = process.StandardOutput.ReadToEnd();
                //Console.WriteLine(output);
            }
    
            var lines = output.Split("\n");
            var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        
            var metrics = new MemoryMetrics();
            metrics.Total = Int64.Parse(memory[1]);
            metrics.Used = Int64.Parse(memory[2]);
            metrics.Free = Int64.Parse(memory[3]);
    
            return metrics;            
        }

        private static MemoryMetrics GetWindowsMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo();
            info.FileName = "wmic";
            info.Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value";
            info.RedirectStandardOutput = true;

            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
            }

            var lines = output.Trim().Split("\n");
            var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetrics();
            metrics.Total = long.Parse(totalMemoryParts[1]);
            metrics.Free = long.Parse(freeMemoryParts[1]);
            metrics.Used = metrics.Total - metrics.Free;

            return metrics;
        }

        private static string ConvertTo(string from, string to, long value)
        {
            string retVal = string.Empty;

            switch (from)
            {
                case MemMeasureType.BIT:
                    {
                        retVal = ConvertFromBitTo(to, value);
                        break;
                    }
                case MemMeasureType.BYTE:
                    {
                        retVal = ConvertFromByteTo(to, value);
                        break;
                    }
                case MemMeasureType.KBIT:
                    {
                        retVal = ConvertFromKBitTo(to, value);
                        break;
                    }
                case MemMeasureType.KBYTE:
                    {
                        retVal = ConvertFromKByteTo(to, value);
                        break;
                    }
                case MemMeasureType.MEGABIT:
                    {
                        retVal = ConvertFromMBitTo(to, value);
                        break;
                    }
                case MemMeasureType.MEGABYTE:
                    {
                        retVal = ConvertFromMByteTo(to, value);
                        break;
                    }
                case MemMeasureType.GIGABYTE:
                    {
                        retVal = ConvertFromGByteTo(to, value);
                        break;
                    }
            }

            return retVal;
        }

        private static string ConvertFromBitTo(string to, long value)
        {
            decimal ret = 0.0M;
            decimal m24 = 1024M;

            switch (to)
            {
                case MemMeasureType.BYTE:
                    {
                        ret = value / 8;
                        break;
                    }
                case MemMeasureType.KBIT:
                    {
                        ret = value / m24;
                        break;
                    }
                case MemMeasureType.KBYTE:
                    {
                        ret = value / 8 / m24;
                        break;
                    }
                case MemMeasureType.MEGABIT:
                    {
                        ret = value / m24 / m24;
                        break;
                    }
                case MemMeasureType.MEGABYTE:
                    {
                        ret = value / 8 / m24 / m24;
                        break;
                    }
                case MemMeasureType.GIGABYTE:
                    {
                        ret = value / 8 / m24 / m24 / 2014;
                        break;
                    }
                default:
                    {
                        ret = decimal.Parse(value.ToString());
                        break;
                    }
            }

            return ret.ToString("0.##");
        }

        private static string ConvertFromKBitTo(string to, long value)
        {
            decimal ret = 0.0M;
            decimal m24 = 1024M;

            switch (to)
            {
                case MemMeasureType.BIT:
                    {
                        ret = value * m24;
                        break;
                    }
                case MemMeasureType.BYTE:
                    {
                        ret = value / 8 / m24;
                        break;
                    }
                case MemMeasureType.KBYTE:
                    {
                        ret = value / 8;
                        break;
                    }
                case MemMeasureType.MEGABIT:
                    {
                        ret = value * m24;
                        break;
                    }
                case MemMeasureType.MEGABYTE:
                    {
                        ret = value * 8 / m24;
                        break;
                    }
                case MemMeasureType.GIGABYTE:
                    {
                        ret = value * 8 / m24 / m24;
                        break;
                    }
                default:
                    {
                        ret = decimal.Parse(value.ToString());
                        break;
                    }
            }

            return ret.ToString("0.##");
        }

        private static string ConvertFromByteTo(string to, long value)
        {
            decimal ret = 0.0M;
            decimal m24 = 1024M;

            switch (to)
            {
                case MemMeasureType.BIT:
                    {
                        ret = value * 8;
                        break;
                    }
                case MemMeasureType.KBIT:
                    {
                        ret = value * 8 * m24;
                        break;
                    }
                case MemMeasureType.KBYTE:
                    {
                        ret = value / m24;
                        break;
                    }
                case MemMeasureType.MEGABIT:
                    {
                        ret = value * 8 / m24 / m24;
                        break;
                    }
                case MemMeasureType.MEGABYTE:
                    {
                        ret = value / m24 / m24;
                        break;
                    }
                case MemMeasureType.GIGABYTE:
                    {
                        ret = value / m24 / m24 / 2014;
                        break;
                    }
                default:
                    {
                        ret = decimal.Parse(value.ToString());
                        break;
                    }
            }

            return ret.ToString("0.##");
        }

        private static string ConvertFromKByteTo(string to, long value)
        {
            decimal ret = 0.0M;
            decimal m24 = 1024M;

            switch (to)
            {
                case MemMeasureType.BIT:
                    {
                        ret = value * 8 * m24;
                        break;
                    }
                case MemMeasureType.BYTE:
                    {
                        ret = value * m24;
                        break;
                    }
                case MemMeasureType.KBIT:
                    {
                        ret = value * 8;
                        break;
                    }
                case MemMeasureType.MEGABIT:
                    {
                        ret = value * 8 / m24;
                        break;
                    }
                case MemMeasureType.MEGABYTE:
                    {
                        ret = value / m24;
                        break;
                    }
                case MemMeasureType.GIGABYTE:
                    {
                        ret = value / m24 / m24;
                        break;
                    }
                default:
                    {
                        ret = decimal.Parse(value.ToString());
                        break;
                    }
            }

            return ret.ToString("0.##");
        }

        private static string ConvertFromMBitTo(string to, long value)
        {
            decimal ret = 0.0M;
            decimal m24 = 1024M;

            switch (to)
            {
                case MemMeasureType.BIT:
                    {
                        ret = value * m24 * m24;
                        break;
                    }
                case MemMeasureType.BYTE:
                    {
                        ret = value / 8 * m24 * m24;
                        break;
                    }
                case MemMeasureType.KBIT:
                    {
                        ret = value * m24;
                        break;
                    }
                case MemMeasureType.KBYTE:
                    {
                        ret = value / 8 * m24;
                        break;
                    }
                case MemMeasureType.MEGABYTE:
                    {
                        ret = value / 8;
                        break;
                    }
                case MemMeasureType.GIGABYTE:
                    {
                        ret = value / 8 / m24;
                        break;
                    }
                default:
                    {
                        ret = decimal.Parse(value.ToString());
                        break;
                    }
            }

            return ret.ToString("0.##");
        }

        private static string ConvertFromMByteTo(string to, long value)
        {
            decimal ret = 0.0M;
            decimal m24 = 1024M;

            switch (to)
            {
                case MemMeasureType.BIT:
                    {
                        ret = value * 8 * m24 * m24;
                        break;
                    }
                case MemMeasureType.BYTE:
                    {
                        ret = value * m24 * m24;
                        break;
                    }
                case MemMeasureType.KBIT:
                    {
                        ret = value * 8 * m24;
                        break;
                    }
                case MemMeasureType.KBYTE:
                    {
                        ret = value * m24;
                        break;
                    }
                case MemMeasureType.MEGABIT:
                    {
                        ret = value / 8;
                        break;
                    }
                case MemMeasureType.GIGABYTE:
                    {
                        ret = value / m24;
                        break;
                    }
                default:
                    {
                        ret = decimal.Parse(value.ToString());
                        break;
                    }
            }

            return ret.ToString("0.##");
        }

        private static string ConvertFromGByteTo(string to, long value)
        {
            decimal ret = 0.0M;
            decimal m24 = 1024M;

            switch (to)
            {
                case MemMeasureType.BIT:
                    {
                        ret = value * 8 * m24 * m24 * m24;
                        break;
                    }
                case MemMeasureType.BYTE:
                    {
                        ret = value * m24 * m24 * m24;
                        break;
                    }
                case MemMeasureType.KBIT:
                    {
                        ret = value * 8 * m24 * m24;
                        break;
                    }
                case MemMeasureType.KBYTE:
                    {
                        ret = value * m24 * m24;
                        break;
                    }
                case MemMeasureType.MEGABIT:
                    {
                        ret = value * 8 * m24;
                        break;
                    }
                default:
                    {
                        ret = decimal.Parse(value.ToString());
                        break;
                    }
            }

            return ret.ToString("0.##");
        }
    }
}
