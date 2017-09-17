using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace plugins
{
    public static class CCCodeGenerator
    {
        public static string Exchs = "BTC38, BTCC, BTER, Bit2C, Bitfinex, Bitstamp, Bittrex, CCEDK, Cexio, Coinbase, Coinfloor, Coinse, Coinsetter, Cryptopia, Cryptsy, Gatecoin, Gemini, HitBTC, Huobi, itBit, Kraken, LakeBTC, LocalBitcoins, MonetaGo, OKCoin, Poloniex, Yacuna, Yunbi, Yobit, Korbit, BitBay, BTCMarkets, QuadrigaCX, CoinCheck, BitSquare, Vaultoro, MercadoBitcoin, Unocoin, Bitso, BTCXIndia, Paymium, TheRockTrading, bitFlyer, Quoine, Luno, EtherDelta, Liqui, bitFlyerFX, BitMarket, LiveCoin, Coinone, Tidex, Bleutrade, EthexIndia";
        public static List<string> ExchangesRaw => Exchs.ToCsv(true);

        public static string Code = "public class {0}CryptoCompareProvider : CryptoCompareBase {{ public override string Name => \"{0}\";}}";

        public static string Build()
        {
            return ExchangesRaw.Aggregate("", (current, s) => current + string.Format(Code, s) + Environment.NewLine);
        }
    }
}