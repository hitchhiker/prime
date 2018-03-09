using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.KeysManager.Core.Models
{
    public class ProviderDetailsModel
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Extra { get; set; }
        public string Id { get; set; }
    }
}
