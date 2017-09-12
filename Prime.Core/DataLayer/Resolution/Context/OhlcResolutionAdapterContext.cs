using System;
using System.Linq;

namespace Prime.Core
{
    public class OhlcResolutionAdapterContext : OhlcProviderFinderContext
    {
        public OhlcResolutionAdapterContext() { }

        public OhlcResolutionAdapterContext(OhlcResolutionAdapterContext ctx) : base(ctx)
        {
            MemoryStorageEnabled = ctx.MemoryStorageEnabled;
            DbStorageEnabled = ctx.DbStorageEnabled;
            StorageEnabled = ctx.StorageEnabled;
            ApiEnabled = ctx.ApiEnabled;
            CanDiscoverApiProviders = ctx.CanDiscoverApiProviders;
            ApiDiscoveryFunction = ctx.ApiDiscoveryFunction;
            Network = ctx.Network;
            TimeResolution = ctx.TimeResolution;
            AssetIntermediary = ctx.AssetIntermediary;
            AssetPegged = ctx.AssetPegged;
            PrimaryApiProvider = ctx.PrimaryApiProvider;
            CurrencyConversionApiProvider = ctx.CurrencyConversionApiProvider;
            ProvidersForDirect = ctx.ProvidersForDirect;
            ProvidersForConversion = ctx.ProvidersForConversion;
            IsDataConverted = ctx.IsDataConverted;
            StatusEntry = ctx.StatusEntry;
        }
        
        public bool MemoryStorageEnabled { get; set; } = true;

        public bool DbStorageEnabled { get; set; } = true;

        public bool StorageEnabled { get; set; } = true;

        public bool ApiEnabled { get; set; } = true;

        public bool CanDiscoverApiProviders { get; set; } = true;

        public Func<(OhlcPairProviders direct, OhlcPairProviders via)> ApiDiscoveryFunction { get; set; }

        public Network Network { get; set; }

        public TimeResolution TimeResolution { get; set; }

        public Asset AssetIntermediary { get; set; }

        public Asset AssetPegged { get; set; }

        public IOhlcProvider PrimaryApiProvider { get; set; }

        public IOhlcProvider CurrencyConversionApiProvider { get; set; }

        public OhlcPairProviders ProvidersForDirect { get; set; }

        public OhlcPairProviders ProvidersForConversion { get; set; }

        public bool IsDataConverted { get; private set; }

        public Action<string> StatusEntry { get; set; }

        public void Status(string entry)
        {
            if (!string.IsNullOrWhiteSpace(entry))
                StatusEntry?.Invoke(entry);
        }

        public bool RequiresApiDiscovery()
        {
            return ApiEnabled && CanDiscoverApiProviders && (PrimaryApiProvider == null || (ConversionEnabled && CurrencyConversionApiProvider == null));
        }

        public void DiscoverAndApplyApiProviders(bool overwrite = false)
        {
            var provs = ApiDiscoveryFunction?.Invoke() ?? new OhlcApiProviderFinder(this).FindProvider();

            ProvidersForConversion = provs.via;
            ProvidersForDirect = provs.direct;
            if (!overwrite)
            {
                PrimaryApiProvider = PrimaryApiProvider ?? ProvidersForDirect?.Provider;
                CurrencyConversionApiProvider = CurrencyConversionApiProvider ?? ProvidersForConversion?.Provider;
            }
            else
            {
                PrimaryApiProvider = ProvidersForDirect?.Provider;
                CurrencyConversionApiProvider = ProvidersForConversion?.Provider;
            }

            if (provs.via != null)
                DoIntermediary(provs.direct, provs.via);

            if (PrimaryApiProvider == null)
                throw new Exception("Cannot locate an api " + nameof(IOhlcProvider) + " for " + Pair);
        }

        private void DoIntermediary(OhlcPairProviders primary, OhlcPairProviders via)
        {
            if (primary.IsPegged)
                AssetPegged = primary.Pair.Asset2;
            else if (via != null && via.IsIntermediary)
                AssetIntermediary = primary.Pair.Asset2;
        }

        public IOhlcProvider GetDefaultAggregationProvider()
        {
            return Networks.I.AssetPairAggregationProviders.OfType<IOhlcProvider>().FirstOrDefault();
        }

        public void EnsureDefaults()
        {
            if (TimeResolution == TimeResolution.None)
                throw new Exception(nameof(TimeResolution) + " must be specified for " + GetType());

            if (Pair == null)
                throw new Exception(nameof(Pair) + " must be specified for " + GetType());
        }

        public void EnsureProvider()
        {
            if (ApiEnabled && PrimaryApiProvider == null)
                throw new Exception("You must specify " + nameof(PrimaryApiProvider) + " for " + nameof(OhlcResolutionAdapterContext));
        }
    }
}