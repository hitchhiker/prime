using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetPairs : IReadOnlyList<AssetPair>, ISerialiseAsObject
    {
        public AssetPairs() { }

        public AssetPairs(IEnumerable<AssetPair> pairs)
        {
            _pairs.AddRange(pairs);
        }

        public AssetPairs(int length, string csv, IWalletService provider)
        {
            foreach (var i in csv.ToCsv(true))
            {
                if (i.Length!= length*2)
                    continue;
                
                var c1 = i.Substring(0, length);
                var c2 = i.Substring(length);
                _pairs.Add(new AssetPair(c1, c2, provider));
            }
        }

        [BsonField("pairs")]
        private UniqueList<AssetPair> _pairs { get; set; } = new UniqueList<AssetPair>();
        
        public void Add(AssetPair pair)
        {
            _pairs.Add(pair);
        }

        [Bson]
        public DateTime UtcLastUpdated { get; set; }

        public IReadOnlyList<Asset> BaseCurrencies() => _pairs.Select(x => x.Asset1).ToUniqueList();

        public int Count => _pairs.Count;

        public IEnumerator<AssetPair> GetEnumerator()
        {
            return _pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _pairs).GetEnumerator();
        }

        public AssetPair this[int index] => _pairs[index];
    }
}