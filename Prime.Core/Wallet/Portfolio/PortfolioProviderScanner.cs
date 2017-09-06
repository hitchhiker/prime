using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Prime.Utility;

namespace Prime.Core.Wallet
{
    public class PortfolioProviderScanner : IDisposable
    {
        public PortfolioProviderScanner(PortfolioProviderScannerContext context)
        {
            L = context.L;
            _context = context.Context;
            Provider = context.Provider;
            BaseAsset = context.BaseAsset;
            _timerFrequency = context.Frequency;
            Info = new PortfolioInfoItem(Provider.Network);
            _providerContext = new NetworkProviderPrivateContext(_context, L);

            if (context.Frequency!=0)
                SetTimer(1);
        }

        public event EventHandler OnChanged;

        private readonly NetworkProviderPrivateContext _providerContext;
        private readonly UserContext _context;
        public readonly IWalletService Provider;
        public readonly Asset BaseAsset;
        private readonly int _timerFrequency;
        public readonly Logger L;
        private Timer _timer;

        public bool IsConnected { get; private set; }
        public bool IsQuerying { get; private set; }
        public bool IsFailing { get; private set; }
        public DateTime UtcLastUpdated { get; private set; }

        public List<PortfolioLineItem> Items { get; } = new List<PortfolioLineItem>();

        public PortfolioInfoItem Info { get; }

        private void SetTimer(int frequency)
        {
            if (frequency == 0)
                return;

            _timer = new Timer(frequency) { AutoReset = false };
            _timer.Elapsed += (o, args) => Update();
            _timer.Start();
        }

        public void Update()
        {
            IsQuerying = true;
            UpdateInfo();

            var r = new List<PortfolioLineItem>();

            try
            {
                BalanceResults apir = null;
                try
                {
                    apir = Provider.GetBalance(_providerContext);
                    IsConnected = true;
                    UpdateInfo();
                }
                catch (Exception e)
                {
                    L.Error(e, $"in {nameof(Update)} @ {nameof(Provider.GetBalance)}");

                    IsConnected = false;
                    UpdateInfo();
                }

                if (apir == null)
                {
                    IsQuerying = false;
                    IsFailing = true;
                    UpdateInfo();
                    return;
                }

                foreach (var br in apir)
                {
                    var li = GetLineItem(br, BaseAsset, Provider.Network);
                    if (li?.Asset == null)
                        continue;
                    
                    r.Add(li);
                    Update(r, false, li);
                }

                IsFailing = false;
                IsQuerying = false;
            }
            catch (Exception e)
            {
                L.Error(e, $"in {nameof(Update)}");
                IsFailing = true;
                IsQuerying = false;
            }

            Update(r, true);

            SetTimer(_timerFrequency);
        }

        private void UpdateInfo()
        {
            var q = Items.Where(x => !x.IsTotalLine);

            Info.IsConnected = IsConnected;
            Info.IsFailed = IsFailing;
            Info.IsQuerying = IsQuerying;
            Info.Assets = q.Select(x => x.Asset).ToUniqueList();
            Info.UtcLastConnect = DateTime.UtcNow;
            Info.TotalConvertedAssetValue = new Money(q.Sum(x => x.Converted), BaseAsset);
        }

        private void Update(List<PortfolioLineItem> r, bool finished, PortfolioLineItem li = null)
        {
            UtcLastUpdated = DateTime.UtcNow;

            if (li != null)
            {
                Items.RemoveAll(x => Equals(x.Asset, li.Asset));
                Items.Add(li);
            }
            else
            {
                Items.Clear();
                foreach (var i in r)
                    Items.Add(i);
            }

            UpdateInfo();

            OnChanged?.Invoke(this, new PortfolioChangedLineEvent(li) {Finished = finished});
        }

        public class PortfolioChangedLineEvent : EventArgs
        {
            public PortfolioChangedLineEvent(PortfolioLineItem li)
            {
                Item = li;
            }

            public PortfolioLineItem Item { get; set; }

            public bool Finished { get; set; }
        }

        private PortfolioLineItem GetLineItem(BalanceResult balance, Asset bAsset, Network i)
        {
            try
            {
                var pair = new AssetPair(balance.Available.Asset, bAsset);
                var fx = pair.Fx() ?? Money.Zero;

                var pli = new PortfolioLineItem()
                {
                    Asset = balance.Asset,
                    Network = i,
                    AvailableBalance = balance.Available,
                    PendingBalance = balance.Reserved,
                    ReservedBalance = balance.Reserved,
                    Total =
                        new Money((decimal) balance.Available + (decimal) balance.Reserved, balance.Available.Asset),
                    Converted = new Money((decimal) balance.Available * (decimal) fx, pair.Asset2),
                    ConversionFailed = fx == Money.Zero,
                    ChangePercentage = 0
                };
                return pli;
            }
            catch (Exception e)
            {
                L.Error(e, $"in {GetType()} @ {nameof(GetLineItem)}");
            }
            return null;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}