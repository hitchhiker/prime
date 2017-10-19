using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Core
{
    public class BookmarkedCommands
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
        public UniqueList<CommandContent> Items { get; private set; } = new UniqueList<CommandContent>();

        public void Add(CommandContent bookmark)
        {
            if (Items.Contains(bookmark))
                return;

            Items.Add(bookmark);
        }

        public void Add(ICanGenerateCommand doc)
        {
            var b = doc.GetPageCommand();
            if (b != null)
                Add(b);
        }

        public void Remove(CommandContent bookmark)
        {
            if (!Items.Contains(bookmark))
                return;

            Items.RemoveAll(x=> x.Equals(bookmark));
        }

        public IEnumerator<CommandContent> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public int Count => Items.Count;
    }
}