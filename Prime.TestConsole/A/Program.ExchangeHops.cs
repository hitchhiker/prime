using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public sealed class ExchangeHops : IReadOnlyList<ExchangeHop>
        {
            private List<ExchangeHop> _routes;

            public ExchangeHops()
            {
                _routes = new List<ExchangeHop>();
            }

            public void Add(ExchangeHop route)
            {
                _routes.Add(route);
                _routes = _routes.OrderByDescending(x => x.Percentage).ToList();
            }

            public IEnumerator<ExchangeHop> GetEnumerator()
            {
                return _routes.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_routes).GetEnumerator();
            }

            public int Count => _routes.Count;

            public ExchangeHop this[int index] => _routes[index];
        }
    }
}