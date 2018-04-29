using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Core.Messages;

namespace Prime.Core
{
    public class AssetAllRequestMessage : RequestorTokenMessageBase
    {
        public AssetAllRequestMessage(string requesterToken) : base(requesterToken)
        {
        }
    }
}
