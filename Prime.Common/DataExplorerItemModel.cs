using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public class DataExplorerItemModel
    {
        public string Title { get; }
        public AssetPair AssetPair { get; }

        public DataExplorerItemModel(string title, AssetPair assetPair)
        {
            this.Title = title;
            this.AssetPair = assetPair;
        }
    }
}
