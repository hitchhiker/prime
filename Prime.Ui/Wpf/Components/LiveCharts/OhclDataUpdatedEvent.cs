using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Ui.Wpf
{
    public class OhclDataUpdatedEvent : EventArgs
    {
        public readonly OhclData NewData;
        public readonly Asset Asset;

        public OhclDataUpdatedEvent(OhclData newData, Asset asset)
        {
            NewData = newData;
            Asset = asset;
        }
    }
}
