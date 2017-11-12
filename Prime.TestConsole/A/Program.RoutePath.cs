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
            public readonly Network StartNetwork;
            private readonly List<RouteStage> _stages = new List<RouteStage>();

            public RoutePath(Network startNetwork)
            {
                StartNetwork = startNetwork;
                Id = ObjectId.Empty;
            }

            public ObjectId Id { get; private set; }

            public void Add(RouteStage stage)
            {
                var last = _stages.LastOrDefault();
                _stages.Add(stage);

                if (last == null)
                    return;

                stage.SetBefore(last);
                last.SetNext(stage);
                Id = string.Join(":", _stages.Select(x => x.Id)).GetObjectIdHashCode();
            }

            public IEnumerator<RouteStage> GetEnumerator()
            {
                return _stages.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable) _stages).GetEnumerator();
            }

            public int Count => _stages.Count;

            public RouteStage this[int index] => _stages[index];

            public bool Has(RouteStage stage)
            {
                return _stages.Any(x => Equals(x.ExchangeHop.NetworkLow, stage.ExchangeHop.NetworkLow));
            }

            public RoutePath Clone()
            {
                var nr = new RoutePath(StartNetwork);
                foreach (var i in _stages)
                    nr.Add(i);
                return nr;
            }

        }
    }
}