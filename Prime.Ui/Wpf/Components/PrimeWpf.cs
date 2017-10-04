using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amib.Threading;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.Utility;

namespace Prime.Ui.Wpf
{
    public class PrimeWpf
    {
        private PrimeWpf() {}

        public static PrimeWpf I => Lazy.Value;
        private static readonly Lazy<PrimeWpf> Lazy = new Lazy<PrimeWpf>(()=>new PrimeWpf());

        private List<IPaneProvider> _panelProviders;
        public IReadOnlyList<IPaneProvider> PanelProviders => _panelProviders ?? (_panelProviders = TypeCatalogue.I.ImplementInstances<IPaneProvider>().ToList());

        private SmartThreadPool _sTaThreadPool;
        public SmartThreadPool STAThreadPool => _sTaThreadPool ?? (_sTaThreadPool = GetStaThreadPool());

        private bool? _isDemo;
        public bool IsDemo => _isDemo ?? (bool) (_isDemo = File.Exists(Path.Combine(CommonFs.I.UserConfigDirectory.FullName, "demo")));

        private SmartThreadPool GetStaThreadPool()
        {
            var stpStartInfo = new STPStartInfo {ApartmentState = ApartmentState.STA};
            return new SmartThreadPool(stpStartInfo);
        }
    }
}
