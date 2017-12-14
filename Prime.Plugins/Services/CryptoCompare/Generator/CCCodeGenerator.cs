using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace plugins
{
    public static class CCCodeGenerator
    {
        public static string Exchs = "Cryptsy, BTCChina, Bitstamp, BTER, OKCoin, Coinbase, Poloniex, Cexio, BTCE, BitTrex, Kraken, Bitfinex, Yacuna, LocalBitcoins, Yunbi, itBit, HitBTC, btcXchange, BTC38, Coinfloor, Huobi, CCCAGG, LakeBTC, ANXBTC, Bit2C, Coinsetter, CCEX, Coinse, MonetaGo, Gatecoin, Gemini, CCEDK, Cryptopia, Exmo, Yobit, Korbit, BitBay, BTCMarkets, Coincheck, QuadrigaCX, BitSquare, Vaultoro, MercadoBitcoin, Bitso, Unocoin, BTCXIndia, Paymium, TheRockTrading, bitFlyer, Quoine, Luno, EtherDelta, bitFlyerFX, TuxExchange, CryptoX, Liqui, MtGox, BitMarket, LiveCoin, Coinone, Tidex, Bleutrade, EthexIndia, Bithumb, CHBTC, ViaBTC, Jubi, Zaif, Novaexchange, WavesDEX, Binance, Lykke, Remitano, Coinroom, Abucoins, BXinth, Gateio, HuobiPro, OKEX";
        public static List<string> ExchangesRaw => Exchs.ToCsv(true);

        public static string Code = "public class {0}CryptoCompareProvider : CryptoCompareBase {{ public override string Name => \"{0}\";}}";

        public static string Build()
        {
            return ExchangesRaw.OrderBy(x=>x).Aggregate("", (current, s) => current + string.Format(Code, s) + Environment.NewLine);
        }
    }
}