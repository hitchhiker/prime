using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core.Misc
{
    public class CoinsItemModel
    {
        public decimal ChangePerc { get; }
        public string IconPath { get; }
        public string Name { get; }
        public string Description { get; }
        public string Algorithm { get; }
        public string ProofType { get; }
        public string MarketCap { get; }
        public string LastTradeDescription { get; }
        public Money Price { get; }
        public Money Volume { get; }
        public DateTime LastTradeTime { get; }

        public CoinsItemModel(decimal changePerc, Money price, Money volume, string iconPath, string name, string description, string algorithm, string proofType, string marketCap, string lastTradeDescription, DateTime lastTradeTime)
        {
            this.ChangePerc = changePerc;
            this.Price = price;
            this.Volume = volume;
            this.IconPath = iconPath;
            this.Name = name;
            this.Description = description;
            this.Algorithm = algorithm;
            this.ProofType = proofType;
            this.MarketCap = marketCap;
            this.LastTradeDescription = lastTradeDescription;
            this.LastTradeTime = lastTradeTime;
        }
    }
}
