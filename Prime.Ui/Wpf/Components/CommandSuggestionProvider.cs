using System.Collections;
using System.Collections.Generic;
using WpfControls;

namespace prime
{

    public class CommandSuggestionProvider : ISuggestionProvider
    {
        public IEnumerable GetSuggestions(string filter)
        {
            return new List<string>() {"ok", "ok2"};
        }
    }
}