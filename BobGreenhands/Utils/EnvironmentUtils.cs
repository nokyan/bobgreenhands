using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;

namespace BobGreenhands.Utils
{
    /// <summary>
    /// Some utility methods to get stuff like CPU information, since .NET Core doesn't provide that OOTB
    /// </summary>
    public static class EnvironmentUtils
    {
        /// <summary>
        /// Returns a string with OS information similar to Java's System.getProperty() stuff
        /// </summary>
        public static string GetFullOSName()
        {
            // BSD or Linux running? Nice, that means we can use the glorious bash and get our information from there!
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string output = "N/A";
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "/bin/bash";
                info.Arguments = "-c \"uname -s -r\"";
                info.RedirectStandardOutput = true;
                using (Process process = Process.Start(info))
                {
                    output = process.StandardOutput.ReadToEnd().Replace("\n", "");
                }
                return String.Format("{0} {1} (running as {2})", output, RuntimeInformation.OSArchitecture, RuntimeInformation.ProcessArchitecture);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {    // macOS/OS X running? Alright, uname is more or less useless since we want the macOS version, not the Darwin kernel version. Use something else.
                string output = "";
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "/bin/bash";
                info.Arguments = "-c \"sw_vers -productName\"";
                info.RedirectStandardOutput = true;
                using (Process process = Process.Start(info))
                {
                    output += process.StandardOutput.ReadToEnd().Replace("\n", " ");
                }
                info.Arguments = "-c \"sw_vers -productVersion\"";
                using (Process process = Process.Start(info))
                {
                    output += process.StandardOutput.ReadToEnd().Replace("\n", " ");
                }
                info.Arguments = "-c \"sw_vers -buildVersion\"";
                using (Process process = Process.Start(info))
                {
                    output += " (Build: " + process.StandardOutput.ReadToEnd().Replace("\n", "") + ")";
                }
                return String.Format("{0} {1} (running as {2})", output, RuntimeInformation.OSArchitecture, RuntimeInformation.ProcessArchitecture);
            }
            // else, this will work just fine.
            return String.Format("{0} {1} (running as {2})", RuntimeInformation.OSDescription, RuntimeInformation.OSArchitecture, RuntimeInformation.ProcessArchitecture);
        }

        public static string GetCPUName()
        {
            string output = "N/A";
            ProcessStartInfo info = new ProcessStartInfo();
            // bash stuff
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                info.FileName = "/bin/bash";
                info.Arguments = "-c \"cat /proc/cpuinfo | grep 'model name' | uniq\"";
                info.RedirectStandardOutput = true;
                using (Process process = Process.Start(info))
                {
                    output = process.StandardOutput.ReadToEnd().Replace("\n", "").Replace("model name\t: ", "");
                }
                return output;
            }
            // also bash stuff but it's macOS/OS X
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                info.FileName = "/bin/bash";
                info.Arguments = "-c \"sysctl -n machdep.cpu.brand_string\"";
                info.RedirectStandardOutput = true;
                using (Process process = Process.Start(info))
                {
                    output = process.StandardOutput.ReadToEnd().Replace("\n", "");
                }
                return output;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                info.FileName = "wmic";
                info.Arguments = "cpu get name";
                info.RedirectStandardOutput = true;
                using (Process process = Process.Start(info))
                {
                    output = process.StandardOutput.ReadToEnd().Split("\n")[1];
                }
                return output;
            }
            // sorry to the 5 people out there using FreeBSD!
            return "N/A";
        }

        /// <summary>
        /// Returns a string with GPU information, works only on Linux or Windows.
        /// </summary>
        public static string GetGPUName()
        {
            string output = "N/A";
            ProcessStartInfo info = new ProcessStartInfo();
            // bash stuff
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                info.FileName = "/bin/bash";
                info.Arguments = "-c \"lspci | grep -i 'vga\\|3d\\|2d'\"";
                info.RedirectStandardOutput = true;
                using (Process process = Process.Start(info))
                {
                    string cmdOutput = process.StandardOutput.ReadToEnd().Replace("\n", "");
                    output = Regex.Replace(cmdOutput, @"[0-9]{1,2}:[0-9]{1,2}.[0-9]{1,2} VGA compatible controller: ", "");
                }
                return output;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                info.FileName = "wmic";
                info.Arguments = "path win32_VideoController get name";
                info.RedirectStandardOutput = true;
                using (Process process = Process.Start(info))
                {
                    output = process.StandardOutput.ReadToEnd().Split("\n")[1];
                }
                return output;
            }
            return "N/A";
        }

        /// <summary>
        /// Returns a string with memory information.
        /// </summary>
        public static string GetMemoryInfo()
        {
            string output = "using {0} MiB, {1}/{2} MiB free ({3:0.0}%)";
            int processusing = (int)Process.GetCurrentProcess().PrivateMemorySize64 / 1048576;
            ProcessStartInfo info = new ProcessStartInfo();
            // bash stuff
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                info.FileName = "/bin/bash";
                info.Arguments = "-c \"LC_ALL=C free -m | awk '/Mem:/ { print $2 } /buffers\\/cache/ { print $3 }' && LC_ALL=C free -m | awk '/Mem:/ { print $7 } /buffers\\/cache/ { print $3 }'\"";
                // command will output the available amount of memory in the first line and the free memory in the second
                info.RedirectStandardOutput = true;
                using (Process process = Process.Start(info))
                {
                    string[] cmdOutputs = process.StandardOutput.ReadToEnd().Split("\n");
                    output = String.Format(output, processusing, cmdOutputs[1], cmdOutputs[0], 100f * Int32.Parse(cmdOutputs[1]) / Int32.Parse(cmdOutputs[0]));
                }
                return output;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                long free;
                long total;
                info.FileName = "wmic";
                info.Arguments = "OS get FreePhysicalMemory";
                info.RedirectStandardOutput = true;
                using (Process process = Process.Start(info))
                {
                    free = Int64.Parse(process.StandardOutput.ReadToEnd().Split("\n")[1]) / 1024;
                }
                info.Arguments = "computersystem get TotalPhysicalMemory";
                using (Process process = Process.Start(info))
                {
                    total = Int64.Parse(process.StandardOutput.ReadToEnd().Split("\n")[1]) / 1048576;
                }
                output = String.Format(output, processusing, free, total, 100 * free / total);
                return output;
            }
            // can't be bothered to do macOS/OS X memory info gathering, can't this OS just be more like Linux? smh
            return "N/A";
        }
    }
}