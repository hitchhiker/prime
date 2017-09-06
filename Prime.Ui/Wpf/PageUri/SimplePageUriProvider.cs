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
            {"portfolio", "/View/Portfolio/Portfolio.xaml"},
            {"services", "/View/Services/Services.xaml"},
            {"wallet", "/View/Portfolio/Wallet.xaml"},
            {"send", "/View/Portfolio/Send.xaml"},
            {"receive", "/View/Portfolio/Receive.xaml"}
        };


        public static DocumentPaneViewModel GetViewModel(IMessenger messenger, ScreenViewModel model, SimpleContentCommand command)
        {
            switch (command.Command)
            {
                case "portfolio":
                    return new PortfolioPaneViewModel(model) { Key = command.Command, Name = command.Title };
                case "wallet":
                    return new WalletViewModel(model) { Key = command.Command, Name = command.Title };
                case "services":
                    return new ServicesPaneViewModel(messenger, model) { Key = command.Command, Name = command.Title };
                case "send":
                    return null;
                case "receive":
                    return new ReceiveViewModel(model) { Key = command.Command, Name = command.Title };
                default:
                    return null;
            }
        }
    }
}