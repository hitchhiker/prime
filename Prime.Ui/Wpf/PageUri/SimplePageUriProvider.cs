using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;

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
            {"portfolio", "/View/Portfolio/Portfolio.xaml"},
            {"services", "/View/Services/Services.xaml"},
            {"wallet", "/View/Portfolio/Wallet.xaml"},
            {"buy sell", "/View/Trade/BuySell.xaml"},
            {"send", "/View/Portfolio/Send.xaml"},
            {"receive", "/View/Portfolio/Receive.xaml"}
        };


        public static DocumentPaneViewModel GetViewModel(IMessenger messenger, ScreenViewModel model, SimpleContentCommand command)
        {
            switch (command.Command)
            {
                case "exchange rates":
                    return new ExchangeRateViewModel(model) { Key = command.Command, Title = command.Title };
                case "portfolio":
                    return new PortfolioPaneViewModel(model) { Key = command.Command, Title = command.Title };
                case "buy sell":
                    return new BuySellViewModel(model) { Key = command.Command, Title = command.Title };
                case "wallet":
                    return new WalletViewModel(model) { Key = command.Command, Title = command.Title };
                case "services":
                    return new ServicesPaneViewModel(messenger, model) { Key = command.Command, Title = command.Title };
                case "send":
                    return null;
                case "receive":
                    return new ReceiveViewModel(model) { Key = command.Command, Title = command.Title };
                default:
                    return null;
            }
        }
    }
}