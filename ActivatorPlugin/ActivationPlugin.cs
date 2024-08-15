using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using BestConsole;

public class ActivationPlugin : IPlugin
{
    private const string SlmgrPath = @"C:\Windows\System32\slmgr.vbs";
    private const string WinrarKeyPath = "rarreg.key";
    private const string DefaultWinrarDir = @"C:\Program Files\WinRAR";
    private const string OfficeBatFilePath = "activator.bat";

    public string CommandName => "activate";
    public string Description => "Activates Windows, WinRAR, or Office.";

    public void Run(string[] args)
    {
        if (!IsAdministrator())
        {
            Console.WriteLine("Please run the program as an administrator.");
            return;
        }

        if (args.Length < 2)
        {
            Console.WriteLine("Usage: activate <winrar/windows/office>");
            return;
        }

        switch (args[1].ToLower())
        {
            case "windows":
                ActivateWindows();
                break;
            case "winrar":
                ActivateWinrar();
                break;
            case "office":
                ActivateOffice();
                break;
            default:
                Console.WriteLine("Unknown parameter. Use winrar, windows, or office.");
                break;
        }
    }

    private void ActivateWindows()
    {
        string[] commands = {
            $"cscript {SlmgrPath} /ipk W269N-WFGWX-YVC9B-4J6C9-T83GX",
            $"cscript {SlmgrPath} /skms kms8.msguides.com",
            $"cscript {SlmgrPath} /ato"
        };

        RunCommands(commands);
    }

    private void ActivateWinrar()
    {
        string destinationDir = DefaultWinrarDir;
        string destinationFile = Path.Combine(destinationDir, WinrarKeyPath);

        string licenseData = @"RAR registration data
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

        try
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            if (File.Exists(destinationFile))
            {
                Console.WriteLine("WinRAR is already activated.");
            }
            else
            {
                File.WriteAllText(destinationFile, licenseData);
                Console.WriteLine("WinRAR activated successfully!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error activating WinRAR: {e.Message}");
        }
    }

    private void ActivateOffice()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = OfficeBatFilePath,
                CreateNoWindow = true,
                UseShellExecute = false
            }).WaitForExit();

            Console.WriteLine("Office activated successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error activating Office: {e.Message}");
        }
    }

    private void RunCommands(string[] commands)
    {
        foreach (var command in commands)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });

            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Command failed: {command}");
                Console.WriteLine($"Error: {process.StandardError.ReadToEnd()}");
                return;
            }
        }

        Console.WriteLine("Operation completed successfully!");
    }

    private bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}

public class DeactivationPlugin : IPlugin
{
    private const string SlmgrPath = @"C:\Windows\System32\slmgr.vbs";
    private const string WinrarKeyPath = "rarreg.key";
    private const string DefaultWinrarDir = @"C:\Program Files\WinRAR";

    public string CommandName => "deactivate";
    public string Description => "Deactivates Windows, WinRAR, or Office.";

    public void Run(string[] args)
    {
        if (!IsAdministrator())
        {
            Console.WriteLine("Please run the program as an administrator.");
            return;
        }

        if (args.Length < 2)
        {
            Console.WriteLine("Usage: deactivate <winrar/windows/office>");
            return;
        }

        switch (args[1].ToLower())
        {
            case "windows":
                DeactivateWindows();
                break;
            case "winrar":
                DeactivateWinrar();
                break;
            case "office":
                DeactivateOffice();
                break;
            default:
                Console.WriteLine("Unknown parameter. Use winrar, windows, or office.");
                break;
        }
    }

    private void DeactivateWindows()
    {
        string[] commands = {
            $"cscript {SlmgrPath} /upk",
            $"cscript {SlmgrPath} /cpky"
        };

        RunCommands(commands);
    }

    private void DeactivateWinrar()
    {
        string destinationDir = DefaultWinrarDir;
        string destinationFile = Path.Combine(destinationDir, WinrarKeyPath);

        try
        {
            if (File.Exists(destinationFile))
            {
                File.Delete(destinationFile);
                Console.WriteLine("WinRAR deactivated successfully!");
            }
            else
            {
                Console.WriteLine("WinRAR is not activated.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deactivating WinRAR: {e.Message}");
        }
    }

    private void DeactivateOffice()
    {
        string architecture = Environment.Is64BitOperatingSystem ? "64bit" : "32bit";
        string initialCommand = architecture == "64bit"
            ? @"cd %ProgramFiles%\Microsoft Office\Office16\"
            : @"cd %ProgramFiles(x86)%\Microsoft Office\Office16\";

        string deactivateProPlusCommand = "cscript ospp.vbs /unpkey:6F7TH";
        string deactivateStandardCommand = "cscript ospp.vbs /unpkey:78VT3";

        string finalCommand = $"{initialCommand} && {deactivateProPlusCommand} && {deactivateStandardCommand}";

        RunCommands(new[] { finalCommand });
    }

    private void RunCommands(string[] commands)
    {
        foreach (var command in commands)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });

            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Command failed: {command}");
                Console.WriteLine($"Error: {process.StandardError.ReadToEnd()}");
                return;
            }
        }

        Console.WriteLine("Operation completed successfully!");
    }

    private bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
