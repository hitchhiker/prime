using Prime.Common;

namespace Prime.Ui.Wpf
{
    public interface IResolutionSource
    {
        /// <summary>
        /// Gets or sets the resolution of the data
        /// </summary>
        TimeResolution Resolution { get; set; }
    }
}