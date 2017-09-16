using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Nito.AsyncEx;
using Prime.Utility;

namespace Prime.Core.Wallet
{
    public class PortfolioProvider : UniqueList<PortfolioLineItem>
    {
        public PortfolioProvider(UserContext context)
        {
            Context = context;
            _scanners = new List<PortfolioProviderScanner>();
            StartScanners(Context);

            context.PropertyChanged += Context_PropertyChanged;
        }

        private readonly List<PortfolioProviderScanner> _scanners;
        private readonly int TimerFrequency = 15000;
        public readonly UserContext Context;
        private readonly object _lock = new object();

        public List<IWalletService> FailingProviders { get; } = new List<IWalletService>();
        public List<IWalletService> WorkingProviders { get; } = new List<IWalletService>();
        public List<IWalletService> QueryingProviders { get; } = new List<IWalletService>();
        public UniqueList<PortfolioInfoItem> PortfolioInfoItems { get; } = new UniqueList<PortfolioInfoItem>();
        public DateTime UtcLastUpdated { get; private set; }

        public event EventHandler OnChanged;

        private void Context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            lock (_lock)
            {
                if (e.PropertyName != nameof(UserContext.BaseAsset))
                    return;

                StopScanners();
                StartScanners(Context);
            }
        }

        private void StartScanners(UserContext context)
        {
            lock (_lock)
            {
                var providers = Networks.I.WalletProviders;
                _scanners.Clear();
                _scanners.AddRange(providers.Select(x => new PortfolioProviderScanner(new PortfolioProviderScannerContext(this.Context, x, context.BaseAsset, TimerFrequency))).ToList());
                _scanners.ForEach(x => x.OnChanged += ScannerChanged);
            }
        }

        private void StopScanners()
        {
            lock (_lock)
            {
                _scanners.ForEach(delegate(PortfolioProviderScanner x)
                {
                    x.OnChanged -= ScannerChanged;
                    x.Dispose();
                });

                this.Clear();
                PortfolioInfoItems.Clear();
                FailingProviders.Clear();
                WorkingProviders.Clear();
                QueryingProviders.Clear();
                UtcLastUpdated = DateTime.MinValue;
                OnChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ScannerChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                var ev = e as PortfolioProviderScanner.PortfolioChangedLineEvent;
                var li = ev?.Item;
                var scanner = sender as PortfolioProviderScanner;

                UpdateScanningStatuses();

                var prov = scanner.Provider;
                UtcLastUpdated = scanner.UtcLastUpdated;

                if (li != null)
                {
                    RemoveAll(x => Equals(x.Network, prov.Network) && Equals(x.Asset, li.Asset));
                    Add(li);
                }
                else
                {
                    RemoveAll(x => Equals(x.Network, prov.Network));
                    foreach (var i in scanner.Items)
                        Add(i);
                }

                PortfolioInfoItems.Add(scanner.Info, true);

                DoTotal(scanner.BaseAsset);

                OnChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void DoTotal(Asset bAsset)
        {
            RemoveAll(x => x.IsTotalLine);
            if (this.Any())
                Add(new PortfolioLineItem() { IsTotalLine = true, Converted = new Money(this.Sum(x=>(decimal)x.Converted), bAsset) });
        }
        
        private void UpdateScanningStatuses()
        {
            lock (_lock)
            {
                WorkingProviders.Clear();
                WorkingProviders.AddRange(_scanners.Where(x => x.IsConnected).Select(x => x.Provider));

                FailingProviders.Clear();
                FailingProviders.AddRange(_scanners.Where(x => x.IsFailing).Select(x => x.Provider));

                QueryingProviders.Clear();
                QueryingProviders.AddRange(_scanners.Where(x => x.IsQuerying).Select(x => x.Provider));
            }
        }
    }
}