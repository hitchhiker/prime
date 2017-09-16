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
        public bool IsLive;

        public OhclDataUpdatedEvent(OhclData newData, Asset asset, bool isLive)
        {
            NewData = newData;
            Asset = asset;
            IsLive = isLive;
        }
    }
}
