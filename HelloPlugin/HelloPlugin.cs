using System;
using BestConsole;

namespace HelloPlugin
{
    public class HelloPlugin : IPlugin
    {
        public string CommandName => "hello";
        public string Description => "Prints a hello message to the console.";

        public void Run(string[] args)
        {
            Console.WriteLine("Hello, this is a sample plugin!");
        }
    }
}
