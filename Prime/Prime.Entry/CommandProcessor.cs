using System;
using System.Collections.Generic;
using System.Linq;

namespace Prime
{
    class CommandProcessor
    {
        private readonly Dictionary<string, Action<string>> _commandBindings = new Dictionary<string, Action<string>>();

        public CommandProcessor()
        {
            _commandBindings.Add("?", s => { PrintCommands(); });
        }

        public void Bind(string command, Action<string> action)
        {
            if(_commandBindings.ContainsKey(command)) 
                throw new InvalidOperationException("Command already added.");

            _commandBindings.Add(command, action);
        }

        private void ProcessCommand()
        {
            Console.Write("> ");
            var command = Console.ReadLine();

            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine("Please enter command.");
                return;
            }

            command = GetCommandByShort(command);

            var key = FindKeyByCommand(command);
            
            if (!_commandBindings.TryGetValue(key, out var action))
            {
                Console.WriteLine("Command not found. Print '?' to list all commands.");
            }

            action?.Invoke(command);
        }

        private string FindKeyByCommand(string command)
        {
            return _commandBindings.Keys.FirstOrDefault(x => command.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        public void Start()
        {
            PrintCommands();

            while (true)
            {
                try
                {
                    ProcessCommand();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unhandled exception occurred.\n" + e.Message);
                }
            }
        }

        private string GetCommandByShort(string input)
        {
            var allCommands = _commandBindings.Keys;

            var abbr = allCommands.ToDictionary((s) =>
                {
                    var parts = s.Split('-');
                    var key = parts.Length > 1 ? parts.Aggregate("", (s1, s2) => s1 + s2.First()) : s;
                    return key;
                },
                s => s);

            return !abbr.TryGetValue(input, out var command) ? input : command;
        }

        private void PrintCommands()
        {
            Console.WriteLine("Supported commands: \n" + string.Join("\n", _commandBindings.Keys));
        }
    }
}
