using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prime.Utility;

namespace Prime.Core
{
    public class BookmarkedCommands : IReadOnlyList<CommandContent>, LiteDB.ISerialiseAsObject
    {
        public void Defaults()
        {
            Items = new UniqueList<CommandContent>
            {
                new AssetGoCommand("BTC".ToAssetRaw()),
                new AssetGoCommand("ETH".ToAssetRaw()),
                new AssetGoCommand("XRP".ToAssetRaw()),
                new AssetGoCommand("LTC".ToAssetRaw())
            };
        }

        [Bson]
        private UniqueList<CommandContent> Items { get; set; }

        public void Add(CommandContent bookmark)
        {
            Items.Add(bookmark);

            if (!_o.Contains(bookmark))
                _o.Add(bookmark);
        }

        public void Add(ICanGenerateCommand doc)
        {
            var b = doc.Create();
            if (b != null)
                Add(b);
        }

        public void Remove(CommandContent bookmark)
        {
            Items.Remove(bookmark);

            if (_o.Contains(bookmark))
                _o.Remove(bookmark);
        }

        private readonly ObservableCollection<CommandContent> _o = new ObservableCollection<CommandContent>();

        public ObservableCollection<CommandContent> InitObservable()
        {
            foreach (var i in Items)
                _o.Add(i);
            return _o;
        }

        public IEnumerator<CommandContent> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Items).GetEnumerator();
        }

        public int Count => Items.Count;

        public CommandContent this[int index] => Items[index];
    }
}