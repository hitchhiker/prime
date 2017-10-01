using Prime.Core;
using Prime.Core.Exchange.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Prime.Ui.Wpf.ViewModel
{
    public class ExchangesViewModel : DocumentPaneViewModel
    {
        public ExchangesViewModel(ScreenViewModel screenViewModel)
        {
            _context = UserContext.Current;
            Dispatcher = Application.Current.Dispatcher;
            random = new Random();
            new Task(PopulateGrid).Start();
        }

        private Random random = null;
        public BindingList<ExchangeItemModel> ListExchangeItems { get; private set; }

        private void PopulateGrid()
        {
            ListExchangeItems = new BindingList<ExchangeItemModel>() {
                new ExchangeItemModel("N/A","Multiple Cryptos" , "../../Asset/img/BitSquare.png", "BitSquare","BitSquare Description"),
                new ExchangeItemModel("China","Multiple Cryptos" , "../../Asset/img/Gatecoin.png", "Gatecoin","Gatecoin Description"),
                new ExchangeItemModel("NZ","Multiple Cryptos" , "../../Asset/img/Cryptopia.png", "Cryptopia","Cryptopia Description"),
                new ExchangeItemModel("US","Multiple Cryptos" , "../../Asset/img/Bittrex.png", "Bittrex","Bittrex Description")
            };
        }

        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;

        public override CommandContent Create()
        {
            return new SimpleContentCommand("exchanges");
        }
    }
}
