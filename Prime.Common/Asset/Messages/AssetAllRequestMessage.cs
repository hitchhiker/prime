using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common.Messages;

namespace Prime.Common
{
    public class AssetAllRequestMessage : RequestorTokenMessageBase
    {
        public AssetAllRequestMessage(string requesterToken) : base(requesterToken)
        {
        }
    }
}
