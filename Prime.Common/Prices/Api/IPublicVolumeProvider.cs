using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicVolumeProvider : IDescribesAssets
    {
        Task<NetworkPairVolume> GetPublicVolumeAsync(PublicVolumeContext context);

        VolumeFeatures VolumeFeatures { get; }
    }
}