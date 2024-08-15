using System;
using System.IO;
using BestConsole;
using Microsoft.Win32;

namespace WinrarPlugin
{
    public class WinrarPlugin : IPlugin
    {
        public string CommandName => "winrar";
        public string Description => "Activate/Deactivate WinRAR (ADMIN ONLY)";

        private const string RegFileName = "rarreg.key";
        private const string RegistrationData = @"
RAR registration data
WinRAR
Unlimited Company License
UID=4b914fb772c8376b3483
641221225034833ce2ce18e48f952846d6c884e0c93c7971855b76
ef0f6f8776949a38aaf860fce6cb5ffde62890079861be57638717
7131ced835ed65cc743d9777f2ea71a8e32c7e593cf66794343565
b41bcf56929486b8bcdac33d50ecf7739960e9b690ccd5cb4bfcc2
aa202fbbdeee1e320c1dbeeea6e42c27fe46f02301e7b1e36cd32f
043a2234f2e5af257b241c5f55b7f09ac37fa029759bc028640001
0267798310fa30782803a80ddc19599bde98a9160a1c3248779404";

        public void Run(string[] args)
        {
            string winrarPath = GetWinrarPath();
            if (string.IsNullOrEmpty(winrarPath))
            {
                Console.WriteLine("WinRAR not found.");
                return;
            }

            string regFilePath = Path.Combine(winrarPath, RegFileName);

            if (args.Length < 2)
            {
                try
                {
                    ActivateWinrar(regFilePath);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Access denied. Please run the command as an administrator.");
                }
            }
            else
            {
                string action = args[1].ToLower();
                try
                {
                    if (action == "activate")
                    {
                        ActivateWinrar(regFilePath);
                    }
                    else if (action == "deactivate")
                    {
                        DeactivateWinrar(regFilePath);
                    }
                    else
                    {
                        Console.WriteLine("Unknown action. Use 'activate' or 'deactivate'.");
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Access denied. Please run the command as an administrator.");
                }
            }
        }

        private void ActivateWinrar(string regFilePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(regFilePath));
            File.WriteAllText(regFilePath, RegistrationData);
            Console.WriteLine("WinRAR activated.");
        }

        private void DeactivateWinrar(string regFilePath)
        {
            if (File.Exists(regFilePath))
            {
                File.Delete(regFilePath);
                Console.WriteLine("WinRAR deactivated.");
            }
            else
            {
                Console.WriteLine("WinRAR is not activated.");
            }
        }

        private string GetWinrarPath()
        {
            // Try to find WinRAR installation path from the registry
            string winrarPath = GetWinrarPathFromRegistry(Registry.CurrentUser) ?? GetWinrarPathFromRegistry(Registry.LocalMachine);
            if (!string.IsNullOrEmpty(winrarPath) && Directory.Exists(winrarPath))
            {
                return winrarPath;
            }

            // Try to find WinRAR installation path from common locations
            string[] possiblePaths = {
                @"C:\Program Files\WinRAR",
                @"C:\Program Files (x86)\WinRAR"
            };

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        private string GetWinrarPathFromRegistry(RegistryKey baseKey)
        {
            using (RegistryKey key = baseKey.OpenSubKey(@"Software\WinRAR"))
            {
                if (key != null)
                {
                    object value = key.GetValue("AppData");
                    if (value != null)
                    {
                        return value.ToString();
                    }
                }
            }
            return null;
        }
    }
}
