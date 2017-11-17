using System;
using System.Collections;
using System.Collections.Generic;
using LiteDB;

namespace Prime.Common
{
    public class VolumeDataExchanges : ModelBase, IReadOnlyList<VolumeDataExchange>, ISerialiseAsObject
    {
        private VolumeDataExchanges() { }

        public VolumeDataExchanges(AssetPair pair)
        {
            Pair = pair;
            Id = pair.Id;
        }

        public bool IsEmpty => Data.Count==0;

        [Bson]
        public AssetPair Pair { get; private set; }

        [Bson]
        private List<VolumeDataExchange> Data { get; set; } = new List<VolumeDataExchange>();

        public void AddRange(VolumeDataExchanges data)
        {
            foreach (var d in data)
                Add(d);
        }

        public void Add(VolumeDataExchange d)
        {
            if (d.Network == null)
                return;

            if (!d.Pair.EqualsOrReversed(Pair))
                throw new ArgumentException($"Cannot add {nameof(VolumeDataExchange)} object to {nameof(VolumeDataExchanges)} as it has the wrong {nameof(AssetPair)} '{d.Pair}'");

            Data.Add(d.Pair.Reversed.Id == Pair.Id ? d.Reversed() : d);
        }

        public IEnumerator<VolumeDataExchange> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Data).GetEnumerator();
        }

        public int Count => Data.Count;

        public VolumeDataExchange this[int index] => Data[index];
    }
}