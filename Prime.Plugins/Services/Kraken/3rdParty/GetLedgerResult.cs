using System.Collections.Generic;

namespace KrakenApi
{
    public class GetLedgerResult
    {
        public Dictionary<string, LedgerInfo> Ledger;
        public int Count;
    }
}