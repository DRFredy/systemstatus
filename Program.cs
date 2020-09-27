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

            Console.WriteLine($"Machine Name: {sysStat.MachineName}");
            Console.WriteLine("==========================");
            Console.WriteLine($"Total Machine Memory: {sysStat.Memory.Total}");
            Console.WriteLine($"Total Used Memory: {sysStat.Memory.Used}");
            Console.WriteLine($"Total Free Memory: {sysStat.Memory.Free}");
            Console.WriteLine($"O.S. Info: {sysStat.OSInfo}");
            Console.WriteLine($"(this) Process Name: {sysStat.Process.Name}");
            Console.WriteLine($"(this) Process Id: {sysStat.Process.Id}");
            Console.WriteLine($"(this) Process Status: {sysStat.Process.Status}");
            Console.WriteLine($"(this) Process Start Time: {sysStat.Process.StartTime}");
            Console.WriteLine($"(this) Process Memory Usage: {sysStat.Process.MemoryUsed}");
        }
    }
}
