using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public sealed class RoutePath : IReadOnlyList<RouteStage>, IUniqueIdentifier<ObjectId>
        {
            private readonly object _lock = new object();
            public readonly Network StartNetwork;
            public readonly Asset StartAsset;
            private readonly List<RouteStage> _stages = new List<RouteStage>();
            private readonly List<ObjectId> _positiveHashes = new List<ObjectId>();
            private readonly List<ObjectId> _negativeHashes = new List<ObjectId>();
            private readonly int _reversedAt = -1;

            public RoutePath(Network startNetwork, Asset startAsset, int reversedAt =-1)
            {
                StartNetwork = startNetwork;
                StartAsset = startAsset;
                _reversedAt = reversedAt;
            }

            public Money FinalBalance()
            {
                var balance = new Money(1, StartAsset);
                foreach (var s in _stages)
                    balance = s.Calculate(IsReversed(s), balance);
                return balance;
            }

            public void Add(RouteStage stage)
            {
                lock (_lock)
                {
                    var last = _stages.LastOrDefault();
                    _stages.Add(stage);

                    if (last == null)
                        return;

                    _id = null;

                    if (_stages.Count > 1)
                    {
                        _negativeHashes.Add(stage.DestinationReversedHash);
                        _positiveHashes.Add(stage.DestinationHash);
                    }
                }
            }

            private volatile ObjectId _id;
            public ObjectId Id
            {
                get
                {
                    lock (_lock)
                        return _id ?? (_id = GetHash());
                }
            }

            private ObjectId GetHash()
            {
                lock (_lock)
                    return _stages.Select(x => x.Id).GetObjectIdHashCode();
            }

            public IEnumerator<RouteStage> GetEnumerator()
            {
                lock (_lock)
                    return _stages.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                lock (_lock)
                    return ((IEnumerable) _stages).GetEnumerator();
            }

            public int Count
            {
                get
                {
                    lock (_lock)
                        return _stages.Count;
                }
            }

            public RouteStage this[int index]
            {
                get
                {
                    lock (_lock)
                        return _stages[index];
                }
            }

            public bool IsReversed(RouteStage stage)
            {
                if (_reversedAt == -1)
                    return false;

                return _stages.IndexOf(stage) >= _reversedAt;
            }

            public bool Has(RouteStage stage)
            {
                lock (_lock)
                    return _stages.Any(x => Equals(x.ExchangeHop.NetworkLow, stage.ExchangeHop.NetworkLow));
            }

            public RoutePath Clone()
            {
                lock (_lock)
                {
                    var nr = new RoutePath(StartNetwork, StartAsset);
                    foreach (var i in _stages)
                        nr.Add(i);
                    nr._id = _id;
                    return nr;
                }
            }

            public string Explain()
            {
                lock (_lock)
                {
                    if (_stages.Count == 0)
                        return null;

                    var balance = new Money(1, FirstStage().AssetTransfer);

                    var st = $"{Environment.NewLine}{Math.Round(GetCompoundPercentWith() - 100, 2)}%";

                    var isreversed = false;
                    for (var index = 0; index < _stages.Count; index++)
                    {
                        var s = _stages[index];
                        if (index >= _reversedAt && _reversedAt != -1)
                            isreversed = true;
                        
                        var pr = s.Explain(index + 1, isreversed, balance);

                        st += pr.Item1;
                        balance = pr.Item2;
                    }
                    return st;
                }
            }

            public decimal GetCompoundPercent(bool reversed = false)
            {
                lock (_lock)
                {
                    decimal percent = 100;

                    foreach (var s in _stages)
                        if (reversed)
                            percent -= s.GetPercentChange(percent);
                        else
                            percent += s.GetPercentChange(percent);

                    return percent;
                }
            }

            public decimal GetCompoundPercentWith()
            {
                lock (_lock)
                {
                    decimal percent = 100;

                    for (var index = 0; index < _stages.Count; index++)
                    {
                        var s = _stages[index];
                        if (_reversedAt!=-1 && index>=_reversedAt)
                            percent -= Math.Round(s.GetPercentChange(percent),3);
                        else
                            percent += Math.Round(s.GetPercentChange(percent),3);
                    }

                    return percent;
                }
            }

            private decimal? _percentagePositive;
            public decimal PercentagePositive
            {
                get
                {
                    lock (_lock)
                        return _percentagePositive ?? (decimal) (_percentagePositive = GetCompoundPercent(false));
                }
            }

            private decimal? _percentageNegative;
            public decimal PercentageNegative
            {
                get
                {
                    lock (_lock)
                        return _percentageNegative ?? (decimal) (_percentageNegative = GetCompoundPercent(true));
                }
            }

            public RouteStage FirstStage()
            {
                lock (_lock)
                    return _stages.Any() ? _stages[0] : null;
            }

            public RouteStage LastStage()
            {
                lock (_lock)
                    return _stages.LastOrDefault();
            }

            public int Match(RoutePath negativePath)
            {
                if (Count < 2 || negativePath.Count < 2)
                    return -1;

                var f = _positiveHashes.FirstOrDefault(x => negativePath._negativeHashes.Contains(x));
                if (f == null)
                    return -1;

                return _positiveHashes.IndexOf(f);
            }
        }
    }
}