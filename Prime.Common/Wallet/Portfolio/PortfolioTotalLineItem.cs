using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Common.Wallet
{
    public class PortfolioTotalLineItem : PortfolioLineItem
    {
        public override bool IsTotalLine => true;

        public UniqueList<PortfolioLineItem> Items { get; set; }

        public override Money? Converted => Items?.Where(x=>!x.IsTotalLine).Select(x=>x.Converted).Sum();
    }
}