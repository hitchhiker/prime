using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Prime.Common.Exchange.Rates;
using Prime.Utility;

namespace Prime.Common.Wallet
{
    internal class PortfolioProvider : IDisposable
    {
        public PortfolioProvider(PortfolioProviderContext context)
        {
            L = context.L;
            _context = context.Context;
            Provider = context.Provider;
            BaseAsset = context.BaseAsset;
            _timerFrequency = context.Frequency;
            NetworkInfo = new PortfolioNetworkInfoItem(Provider.Network);
            _providerContext = new NetworkProviderPrivateContext(_context, L);

            if (context.Frequency!=0)
                SetTimer(1);
        }

        public event EventHandler OnChanged;

        private readonly NetworkProviderPrivateContext _providerContext;
        private readonly UserContext _context;
        public readonly IBalanceProvider Provider;
        public readonly Asset BaseAsset;
        private readonly int _timerFrequency;
        public readonly ILogger L;
        private Timer _timer;

        public bool IsConnected { get; private set; }
        public bool IsQuerying { get; private set; }
        public bool IsFailing { get; private set; }
        public DateTime UtcLastUpdated { get; private set; }

        public List<PortfolioLineItem> Items { get; } = new List<PortfolioLineItem>();

        public PortfolioNetworkInfoItem NetworkInfo { get; }

        private void SetTimer(int frequency)
        {
            if (frequency == 0 || _isDisposed)
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
                BalanceResults results = null;
                try
                {
                    var ar = ApiCoordinator.GetBalances(Provider, _providerContext);
                    IsConnected = ar.Success;

                    if (!IsConnected)
                        L.Error(ar.FailReason ?? "Failed", $"in {nameof(Update)} @ {nameof(Provider.GetBalancesAsync)}");
                    else
                        results = ar.Response;
                    
                    UpdateInfo();
                }
                catch (Exception e)
                {
                    L.Error(e, $"in {nameof(Update)} @ {nameof(Provider.GetBalancesAsync)}");
                    IsConnected = false;
                    UpdateInfo();
                }

                if (results == null)
                {
                    IsQuerying = false;
                    IsFailing = true;
                    UpdateInfo();
                    return;
                }

                foreach (var br in results)
                {
                    var li = CreateLineItem(br, Provider);
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

            NetworkInfo.IsConnected = IsConnected;
            NetworkInfo.IsFailed = IsFailing;
            NetworkInfo.IsQuerying = IsQuerying;
            NetworkInfo.Assets = q.Select(x => x.Asset).ToUniqueList();
            NetworkInfo.UtcLastConnect = DateTime.UtcNow;
        }

        private void Update(List<PortfolioLineItem> r, bool finished, PortfolioLineItem li = null)
        {
            UtcLastUpdated = DateTime.UtcNow;

            if (li != null)
            {
                AddOrUpdate(li);
            }
            else
            {
                foreach (var i in r)
                    AddOrUpdate(i);
            }

            UpdateInfo();

            OnChanged?.Invoke(this, new PortfolioChangedLineEvent(li) {Finished = finished});
        }

        private void AddOrUpdate(PortfolioLineItem newItem)
        {
            var eli = Items.FirstOrDefault(x => Equals(x.Asset, newItem.Asset));
            if (eli == null)
                Items.Add(newItem);
            else
                eli.Update(newItem);
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

        private PortfolioLineItem CreateLineItem(BalanceResult balance, IBalanceProvider provider)
        {
            try
            {
                var pli = new PortfolioLineItem()
                {
                    Asset = balance.Asset,
                    Network = provider.Network,
                    AvailableBalance = balance.Available,
                    PendingBalance = balance.Reserved,
                    ReservedBalance = balance.Reserved,
                    Total = new Money((decimal) balance.Available + (decimal) balance.Reserved, balance.Available.Asset),
                    ChangePercentage = 0
                };

                return pli;
            }
            catch (Exception e)
            {
                L.Error(e, $"in {GetType()} @ {nameof(CreateLineItem)}");
            }
            return null;
        }

        private bool _isDisposed;
        public void Dispose()
        {
            _isDisposed = true;
            _timer?.Dispose();
        }
    }
}