using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BestConsole;

namespace PluginManager
{
    public class PluginManager : IPlugin
    {
        public string CommandName => "plugin";
        public string Description => "Manages plugins with the list command.";

        public void Run(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: plugin <command>");
                return;
            }

            string command = args[1].ToLower();

            switch (command)
            {
                case "list":
                    ListPlugins();
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }

        private void ListPlugins()
        {
            string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            if (!Directory.Exists(pluginsPath))
            {
                Console.WriteLine("No plugins directory found.");
                return;
            }

            var pluginFiles = Directory.GetFiles(pluginsPath, "*.dll");
            if (pluginFiles.Length == 0)
            {
                Console.WriteLine("No plugins found.");
                return;
            }

            Console.WriteLine("Installed plugins:");
            foreach (var file in pluginFiles)
            {
                Console.WriteLine(Path.GetFileName(file));
            }
        }
    }
}
