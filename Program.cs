using System;
using ProcessInfo.Models;
using ProcessInfo.Helpers;

namespace ProcessInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessInfoHelper procHelper = new ProcessInfoHelper();
            
            SystemStatus sysStat = procHelper.GetSystemInfo();

            Console.WriteLine($"Machine Name: .. {sysStat.MachineName}");
            Console.WriteLine($"O.S. Info: ..... {sysStat.OSInfo}");
            Console.WriteLine($"Architecture: .. {sysStat.Architecture}");
            Console.WriteLine();
            Console.WriteLine("Memory:");
            Console.WriteLine($"  Total: ....... {sysStat.Memory.Total}");
            Console.WriteLine($"  Used: ........ {sysStat.Memory.Used}");
            Console.WriteLine($"  Free: ........ {sysStat.Memory.Free}");
            Console.WriteLine();
            Console.WriteLine("Process (this):");
            Console.WriteLine($"  Name: ........ {sysStat.Process.Name}");
            Console.WriteLine($"  Id: .......... {sysStat.Process.Id}");
            Console.WriteLine($"  Status: ...... {sysStat.Process.Status}");
            Console.WriteLine($"  Start Time: .. {sysStat.Process.StartTime}");
            Console.WriteLine($"  Mem. Usage: .. {sysStat.Process.MemoryUsed}");
        }
    }
}
