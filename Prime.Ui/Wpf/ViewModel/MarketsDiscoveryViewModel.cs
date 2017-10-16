﻿using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Prime.Core;
using Prime.Core.Exchange.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class MarketsDiscoveryViewModel : DocumentPaneViewModel, ICanMore, INotifyPropertyChanged
    {
        public MarketsDiscoveryViewModel(ScreenViewModel screenViewModel)
        {
            _context = UserContext.Current;
            Dispatcher = Application.Current.Dispatcher;
            ListMarketControls = new BindingList<MarketControlViewModel>();
            ListSortBy = new BindingList<string>() { "Volume", "Popularity" };
            AddRequest(0, 2);

            _messenger.Register<BookmarkSetMessage>(this, _context.Token, UpdateStar);

            StarCommand = new RelayCommand(() =>
            {
               // _messenger.Send(new BookmarkSetMessage(/*Not sure what goes here*/, isBookmarked), _context.Token);
            });
        }

        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;

        public RelayCommand StarCommand { get; }

        public string StarPath { get; private set; }
        public BindingList<string> ListSortBy { get; private set; }
        public BindingList<MarketControlViewModel> ListMarketControls { get; private set; }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("markets discovery");
        }

        private static readonly Dictionary<string, string> TestItems = new Dictionary<string, string>()
        {
            {"../../Asset/img/BTC.png", "BTC"},
            {"../../Asset/img/DASH.png", "DASH"},
            {"../../Asset/img/ETC.png", "ETC"},
            {"../../Asset/img/ETH.png", "ETHEREUM"},
            {"../../Asset/img/LTC.png", "LTC"},
            {"../../Asset/img/XRP.png", "XRP"}
        };

        private void UpdateStar(BookmarkSetMessage m)
        {
            Dispatcher.Invoke(() =>
            {
                if (m.IsBookmarked)
                {
                    StarPath = "../../Asset/img/Star.png";
                }
                else
                {
                    StarPath = "../../Asset/img/Unstar.png";
                }
            });
        }

        public void AddRequest(int currentPageIndex, int pageSize)
        {
            Random random = new Random();

            for (var i = 0; i < pageSize; i++)
            {
                var ti = TestItems.GetRandom();
                ListMarketControls.Add(new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(random.NextDouble() * (2 + 2) - 2), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), ti.Key, ti.Value)));
            }
        }
    }
}
