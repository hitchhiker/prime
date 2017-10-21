using System;
using Prime.Common;

namespace Prime.Ui.Wpf
{
    public class ResolutionSourceProvider : IResolutionSource
    {
        private readonly Func<TimeResolution> _resolutionProvider;

        public ResolutionSourceProvider(Func<TimeResolution> getResolution)
        {
            _resolutionProvider = getResolution;
        }

        public ResolutionSourceProvider(TimeResolution resolution)
        {
            _resolutionProvider = () => resolution;
        }

        TimeResolution IResolutionSource.Resolution { get => _resolutionProvider.Invoke(); set => throw new NotImplementedException(); }
    }
}