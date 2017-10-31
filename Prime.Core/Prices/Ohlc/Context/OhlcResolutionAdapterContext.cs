using Prime.Common;
using Prime.Utility;
using System;
using System.Linq;

namespace Prime.Core
{
    public class OhlcResolutionAdapterContext
    {
        public OhlcResolutionAdapterContext() { }

        public OhlcResolutionAdapterContext(OhlcResolutionAdapterContext ctx)
        {
            MemoryStorageEnabled = ctx.MemoryStorageEnabled;
            DbStorageEnabled = ctx.DbStorageEnabled;
            StorageEnabled = ctx.StorageEnabled;
            ApiEnabled = ctx.ApiEnabled;
            CanDiscoverApiProviders = ctx.CanDiscoverApiProviders;
            AssetPairProviders = ctx.AssetPairProviders;
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
            Network = ctx.Network;
            Pair = ctx.Pair;
            PeggedEnabled = ctx.PeggedEnabled;
            ConversionEnabled = ctx.ConversionEnabled;
            ReversalEnabled = ctx.ReversalEnabled;
        }

        public Network Network { get; set; }

        public AssetPair Pair { get; set; }

        public bool PeggedEnabled { get; set; }

        public bool ConversionEnabled { get; set; } = true;

        public bool ReversalEnabled { get; set; } = true;

        public bool MemoryStorageEnabled { get; set; } = true;

        public bool DbStorageEnabled { get; set; } = true;

        public bool StorageEnabled { get; set; } = true;

        public bool ApiEnabled { get; set; } = true;

        public bool CanDiscoverApiProviders { get; set; } = true;

        public AssetPairProviders AssetPairProviders { get; set; }

        public TimeResolution TimeResolution { get; set; }

        public Asset AssetIntermediary { get; set; }

        public Asset AssetPegged { get; set; }

        public IOhlcProvider PrimaryApiProvider { get; set; }

        public IOhlcProvider CurrencyConversionApiProvider { get; set; }

        public AssetPairProviders ProvidersForDirect { get; set; }

        public AssetPairProviders ProvidersForConversion { get; set; }

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

        public void ApplyApiProviders()
        {
            var provs = AssetPairProviders;

            ProvidersForDirect = provs;
            ProvidersForConversion = provs.Via;

            PrimaryApiProvider = ProvidersForDirect?.Providers.FirstProviderOf<IOhlcProvider>();
            CurrencyConversionApiProvider = ProvidersForConversion?.Providers.FirstProviderOf<IOhlcProvider>();

            if (provs.Via != null)
                DoIntermediary(provs);

            if (PrimaryApiProvider == null)
                throw new Exception("Cannot locate " + nameof(IOhlcProvider) + " for " + Pair);
        }

        private void DoIntermediary(AssetPairProviders providers)
        {
            if (providers.IsPegged)
                AssetPegged = providers.Pair.Asset2;
            else if (providers.Via != null && providers.Via.IsIntermediary)
                AssetIntermediary = providers.Pair.Asset2;
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