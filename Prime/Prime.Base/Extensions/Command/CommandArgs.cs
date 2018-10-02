using System;
using System.Linq;
using Prime.Core;

namespace Prime.Base
{
    public class CommandArgs : UniqueList<CommandArg>
    {
        public void Process(string[] args)
        {
            var arg = this.FirstOrDefault(x => args.Any(a => a.Equals(x.Verb, StringComparison.OrdinalIgnoreCase)));
            if (arg?.Func == null)
            {
                WriteHelp();
                return;
            }

            arg.Func.Invoke(args);
        }
        
        public void WriteHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("Error:\tUnable to start as no arguments were specified.");
            Console.WriteLine("");
            foreach (var arg in this)
            {
                Console.WriteLine("{0,-15}{1, 0}", "  " + arg.Verb.ToLower(), arg.Help);
                Console.WriteLine();
            }
        }

        public static string[] RemoveItem(string[] args, string removeItem)
        {
            return args.Where(x => !x.Equals(removeItem, StringComparison.OrdinalIgnoreCase)).ToArray();
        }
    }
}
