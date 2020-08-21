using System.Threading;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Nez;
using BobGreenhands.Utils;
using System.Diagnostics;
using BobGreenhands.Scenes;


namespace BobGreenhands
{
    /// <summary>
    /// just a regular ExceptionHandler, saves the crash report and shows the user that something went wrong.
    /// </summary>
    public class ExceptionHandler
    {
        public static Game? Game;

        public static void Crash(object sender, UnhandledExceptionEventArgs e)
        {
            string template = "[[[ CRASH LOG ]]]\n{0} {1}\n\n=== System information ===\nTime: {2}\nOS: {3}\nCPU: {4}\nMemory: {5}\nGPU: {6}\n\n=== Game information ===\nResolution: {7}\nFullscreen: {8}\nCurrent Scene: {9}\nCurrent Savegame: {10}\nLogfile: {11}\n\n=== Stacktrace ===\n{12}\n\n=== Further information ===\nI'm so sorry for this inconvenience and hope that this issue won't happen again. Though not all hope is lost: all the necessary information has been collected in this file and you can even help to resolve this issue if it persists!\n**In case your game hasn't been modded**, please forward this as an issue (including this file and the affected savegame if the crash happend mid-game) on the GitLab repo of the game under https://github.com/ManicRobot/bobgreenhands, you can find more infos in the \"Reporting bugs\" section. Otherwise, forward this issue to the modder(s) - and not me.";

            string time = DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
            string OS = EnvironmentUtils.GetFullOSName();
            string CPU = EnvironmentUtils.GetCPUName();
            string mem = EnvironmentUtils.GetMemoryInfo();
            string GPU = EnvironmentUtils.GetGPUName();
            string res = String.Format("{0}x{1}", Screen.Width, Screen.Height);
            bool fullscreen = Screen.IsFullscreen;
            string? currentScene = Game.Scene.ToString();
            var currentSavegame = "N/A";
            if (PlayScene.CurrentSavegame != null)
            {
                currentSavegame = PlayScene.CurrentSavegame.Path;
            }
            string? stacktrace = e.ExceptionObject.ToString();

            string output = String.Format(template, Program.Name, Program.Version.ToString(), time, OS, CPU, mem, GPU, res, fullscreen, currentScene, currentSavegame, Program.LogFile, stacktrace);
            Console.WriteLine(output);
            string crashlogPath = Path.Combine(Game.GameFolder.CrashlogsFolder, DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + ".txt");
            File.WriteAllText(crashlogPath, output);
            // wait until the file write has completed(?)
            Thread.Sleep(1250);
            OpenFile(crashlogPath);
        }

        /// <summary>
        /// Method that opens any given file using the appropriate program set by the operating system. Works on Linux, OS X/macOS and Windows.
        /// Found on https://github.com/dotnet/corefx/issues/10361
        /// </summary>
        public static void OpenFile(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", String.Format("/c start \"{0}\"", path)));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", String.Format("\"{0}\"", path));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", String.Format("\"{0}\"", path));
            }
        }
    }
}