using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common.Portfolio
{
    public class DesignPortfolioItemModel
    {
        public decimal Units { get; }
        public decimal AvgOpen { get; }
        public decimal ProfitLossPerc { get; }
        public decimal Sell { get; }
        public decimal Buy { get; }
        public string IconPath { get; }
        public string Market { get; }
        public string MarketDescription { get; }
        public Money Invested { get; }
        public Money ProfitLoss { get; }
        public Money Value { get; }

        public DesignPortfolioItemModel(decimal units, decimal avgOpen, decimal profitLossPerc, decimal sell, decimal buy, string iconPath, string market, string marketDescription, Money invested, Money profitLoss, Money value)
        {
            this.Units = units;
            this.AvgOpen = avgOpen;
            this.ProfitLossPerc = profitLossPerc;
            this.Sell = sell;
            this.Buy = buy;
            this.IconPath = iconPath;
            this.Market = market;
            this.MarketDescription = marketDescription;
            this.Invested = invested;
            this.ProfitLoss = profitLoss;
            this.Value = value;
        }
    }
}
