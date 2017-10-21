using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common.Exchange.Model
{
    public class ExchangeItemModel
    {
        public string IconPath { get; }
        public string Name { get; }
        public string Description { get; }
        public string Country { get; }
        public string Type { get; }

        public ExchangeItemModel(string country, string type, string iconPath, string name, string description)
        {
            this.Country = country;
            this.Type = type;
            this.IconPath = iconPath;
            this.Name = name;
            this.Description = description;
        }
    }
}
