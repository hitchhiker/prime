﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Prime.Core
{
    public class ServiceLineItem
    {
        public ServiceLineItem()
        {
            Title = global::Prime.Utility.RandomText.Generate(10);
            Description = global::Prime.Utility.RandomText.Generate(30);
        }

        private readonly INetworkProvider _provider;

        public ServiceLineItem(INetworkProvider provider)
        {
            _provider = provider;
            Title = _provider.Title;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public double Volume { get; set; }

        public INetworkProvider Provider => _provider;
    }
}
