using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System;
using Semver;
using System.Threading.Tasks;
using NLog;
using CommandLine;
using BobGreenhands.Persistence;


namespace BobGreenhands
{
    public static class Program
    {

        public class Options
        {
            [Option('d', "debug", Required = false, HelpText = "Starts the game with debug mode enabled.")]
            public bool Debug { get; set; }

            [Option('f', "folder", Required = false, HelpText = "Specifies the game folder, overwriting the default one.")]
            public string? CustomGameFolder { get; set; }
        }

        public static SemVersion Version
        {
            get; set;
        }

        public static string LogFile;

        public static readonly string Name = "Bob Greenhands";
        public static readonly string OSInformation = Utils.EnvironmentUtils.GetFullOSName();
        public static readonly string CPUInformation = Utils.EnvironmentUtils.GetCPUName();
        public static readonly string GPUInformation = Utils.EnvironmentUtils.GetGPUName();
        public static string? MemInformation;

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        [STAThread]
        static void Main(string[] args)
        {
            // define our version :)
            Version = new SemVersion(0, 0, 1, "alpha");

            // parse arguments
            var arguments = Parser.Default.ParseArguments<Options>(args);
            string folder = "";
            bool debug = false;

            // if a custom game folder is given, use that, if not, use the default one. also set the debug bool
            arguments.WithParsed<Options>(o =>
            {
                debug = o.Debug;
                if (o.CustomGameFolder != "" && o.CustomGameFolder != null && o.CustomGameFolder != "\n" && o.CustomGameFolder != " ")
                {
                    folder = o.CustomGameFolder;
                }
                else
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bob Greenhands");
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                    {
                        folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"/Bob Greenhands";
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"/Application Support/Bob Greenhands";
                    }
                }
            });
            GameFolder gameFolder = new GameFolder(folder);

            LogFile = Path.Combine(gameFolder.LogFolder, DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + ".txt");

            // configure our logger
            var config = new NLog.Config.LoggingConfiguration();
            var logConsole = new NLog.Targets.ConsoleTarget("logConsole");
            var logFile = new NLog.Targets.FileTarget("logFile") { FileName = LogFile };

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);

            LogManager.Configuration = config;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _log.Warn("While Bob Greenhands does run on macOS/OS X, it is currently not supported by the developer. You can report issues on the GitLab repo, though support might be subpar as testing on macOS/OS X is currently not possible.");
            }

            // we don't want to access memory information hundreds of times a second since this would slow down the game quite a lot, so just run a task that will refresh it every second
            Task task = new Task(() =>
            {
                {
                    while (true)
                    {
                        MemInformation = Utils.EnvironmentUtils.GetMemoryInfo();
                        Thread.Sleep(1000);
                    }
                }
            });
            task.Start();

            using (var game = new Game(gameFolder, gameFolder.Settings.GetInt("width"), gameFolder.Settings.GetInt("height"), gameFolder.Settings.GetBool("fullscreen"), gameFolder.Settings.GetBool("vsync"), debug))
            {
                _log.Info("Starting " + Name + " " + Version);
                // set the crash handler
                ExceptionHandler.Game = game;
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler.Crash);
                game.Run();
            }
        }
    }
}
