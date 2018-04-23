using System.Collections.Generic;

namespace Prime.Radiant.Components
{
    public class CloudFlareResponseBase
    {
        public bool success;
        public List<string> errors;
        public List<string> messages;
    }
}