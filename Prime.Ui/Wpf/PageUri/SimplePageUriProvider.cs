using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;
using Prime.Ui.Wpf.ViewModel.Trading;

namespace Prime.Ui.Wpf.PageUri
{
    public class SimplePageUriProvider : IPageUriProvider
    {
        public bool Disabled { get; } = false;

        public GetUriResponse? GetUri(CommandContent commandContent)
        {
            if (commandContent is SimpleContentCommand uri)
                return Commands.Get(uri.Command);
            return null;
        }
        
        private static readonly Dictionary<string, string> Commands = new Dictionary<string, string>()
        {
            {"exchange rates", "/View/Exchange/ExchangeRates.xaml"},
            {"portfolio", "/View/Portfolio/PortfolioNew.xaml"},
            {"services", "/View/Services/Services.xaml"},
            {"wallet", "/View/Portfolio/Wallet.xaml"},
            {"buy sell", "/View/Trade/BuySell.xaml"},
            {"send", "/View/Portfolio/Send.xaml"},
            {"receive", "/View/Portfolio/Receive.xaml"},
            {"watchlist", "/View/Watchlist/Watchlist.xaml"},
            {"coins", "/View/Misc/Coins.xaml"},
            {"exchanges", "/View/Exchange/Exchanges.xaml"},
            {"markets discovery", "/View/Markets/MarketsDiscovery.xaml"}
        };

        public static DocumentPaneViewModel GetViewModel(IMessenger messenger, ScreenViewModel model, SimpleContentCommand command)
        {
            DocumentPaneViewModel dpvm = null;
            model.UiDispatcher.Invoke(() =>
            {
                dpvm = GetViewModelInvoked(model, command);
            });
            return dpvm;
        }

        private static DocumentPaneViewModel GetViewModelInvoked(ScreenViewModel model, SimpleContentCommand command)
        {
            switch (command.Command)
            {
                case "exchange rates":
                    return new ExchangeRateViewModel() { Key = command.Command, Title = command.Title };
                case "portfolio":
                    return new PortfolioPaneViewModel(model) { Key = command.Command, Title = command.Title };
                case "watchlist":
                    return new WatchlistViewModel(model) { Key = command.Command, Title = command.Title };
                case "buy sell":
                    return new BuySellViewModel(model) { Key = command.Command, Title = command.Title };
                case "wallet":
                    return new WalletViewModel() { Key = command.Command, Title = command.Title };
                case "services":
                    return new ServicesPaneViewModel() { Key = command.Command, Title = command.Title };
                case "send":
                    return null;
                case "exchanges":
                    return new ExchangesViewModel(model) { Key = command.Command, Title = command.Title };
                case "coins":
                    return new CoinsViewModel(model) { Key = command.Command, Title = command.Title };
                case "markets discovery":
                    return new MarketsDiscoveryViewModel(model) { Key = command.Command, Title = command.Title };
                case "receive":
                    return new ReceiveViewModel() { Key = command.Command, Title = command.Title };
                default:
                    return null;
            }
        }
    }
}