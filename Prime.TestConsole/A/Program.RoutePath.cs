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
            private readonly List<RouteStage> _stages = new List<RouteStage>();

            public RoutePath(Network startNetwork)
            {
                StartNetwork = startNetwork;
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

            public bool Has(RouteStage stage)
            {
                lock (_lock)
                    return _stages.Any(x => Equals(x.ExchangeHop.NetworkLow, stage.ExchangeHop.NetworkLow));
            }

            public RoutePath Clone()
            {
                lock (_lock)
                {
                    var nr = new RoutePath(StartNetwork);
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

                    var r = $"{Environment.NewLine} +{Math.Round(GetCompoundPercent() - 100, 2)}%";

                    var step = 1;
                    foreach (var s in _stages)
                        r += s.Explain(step++);

                    var last = _stages.LastOrDefault();
                    r +=
                        $"{Environment.NewLine} == [Completed as {last.AssetFinal} @ {last.ExchangeHop.High.Network.Name} ]";

                    return r;
                }
            }

            public decimal GetCompoundPercent()
            {
                lock (_lock)
                {
                    decimal percent = 100;

                    foreach (var s in _stages)
                        percent += s.GetPercentChange(percent);

                    return percent;
                }
            }
        }
    }
}