using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using BestConsole; 

namespace Plugins
{
    public class YoutubeDownloadPlugin : IPlugin
    {
        public string CommandName => "ytdownload";
        public string Description => "Download video/audio from YouTube";

        public void Run(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ytdownload <URL> [format]");
                return;
            }

            string url = args[1];
            string format = args.Length > 2 ? args[2].ToLower() : "mp4";

            if (format != "mp4" && format != "mp3")
            {
                Console.WriteLine("Invalid format. Only 'mp4' and 'mp3' are supported.");
                return;
            }

            string outputDir = GetDownloadsPath();
            Directory.CreateDirectory(outputDir);

            string outputTemplate = Path.Combine(outputDir, "%(title)s.%(ext)s");

            string ytdlpArgs = format == "mp3"
                ? $"-x --audio-format mp3 -o \"{outputTemplate}\" \"{url}\""
                : $"-f \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/mp4\" -o \"{outputTemplate}\" \"{url}\"";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "yt-dlp.exe",
                Arguments = ytdlpArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }

        private static string GetDownloadsPath()
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(homePath, "Downloads");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Path.Combine(homePath, "Downloads");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Path.Combine(homePath, "Downloads");
            }
            else
            {
                throw new NotSupportedException("Unsupported OS platform");
            }
        }
    }
}
