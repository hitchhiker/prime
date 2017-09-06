using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prime.Core;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using plugins;
using prime;
using Prime.Utility;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class ConsoleView
    {
        public ConsoleView()
        {
            InitializeComponent();
            Writer = new TextBoxStreamWriter(Tb1, Tb1, Dispatcher);
            StartButton.Click += StartButton_Click;
        }

        private TextBoxStreamWriter _writer => Writer;

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var ctx = UserContext.Current;
            
            /*
            foreach (var network in NetworkServices.I)
                _writer.WriteLine("Network: " + network.Name + " Data: " + network.NetworkData.Id + " Exchange: " + network.ExchangeData.Id);

           
            foreach (var es in NetworkServices.I.Exchanges.OfType<IWalletService>())
            {
                var b = es.GetBalance(ctx);
                _writer.WriteLine(es.NetworkService.Name + " : " + Environment.NewLine + string.Join(Environment.NewLine, b.Select(x=>x)));
            }



            L.SetLogOutput = s => _writer.WriteLine(s);

            var cc = new CryptoCompareProvider();

            var x = PublicContext.I.PubData.AssetExchangeData(new AssetPair("BTC", "USD", cc));
            L.Trace("Exchanges for " + x.AssetPair + ": " + string.Join(", ", x.Exchanges.Select(e1=>e1.Network.Name)));

            var sp = Networks.I.Providers.FirstOf<CryptoCompareProvider>();
            sp.SubscribePrice((m, lpd) =>
            {
                _writer.WriteLine(m);
            });*/

            // _writer.WriteLine(CCCodeGenerator.Build());
            /*
            
            */
            /*ENCRYPTION*/

            /*var fi = new FileInfo("../../../test.txt");
            if (!fi.Exists)
                File.WriteAllText(fi.FullName, "TEST");

            fi.Encrypt(ctx).Decrypt(ctx);*/

            /*END ENCRYPTION*/
            /*
            foreach (var cn in AssetInfos.Get().Items)
                L.Trace(cn.Asset.Name + ": " + cn.FullName);

            var apik = ctx.As<ProviderData>().ToList().SelectMany(x => x.ApiKeys).ToList();

            L.Info("Api Keys: " + string.Join(", ", apik.Select(x => x.Name)));

            foreach (var es in Networks.I.ExchangeProviders)
            {
                var data = es.Data(ctx);
                data.KeepFresh(ctx, es);
                L.Info(es.Network.Name + " : " + string.Join(", ", data.Assets.Select(x => x.AssetInfo.FullName)));
            }

            foreach (var es in Networks.I.WalletProviders)
            {
                var c = "BTC".ToAsset(es);
                var w = ctx.GetLatestDepositAddress(es, c);
                L.Info(es.Network.Name + " : " + c + " : " + w);
            }
            */
            //L.Info("Complete");
        }

        public TextBoxStreamWriter Writer { get; }
    }
}
