using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicVolumeProvider
    {
        VolumeFeatures VolumeFeatures { get; }

        Task<NetworkPairVolume> GetVolumeAsync();
    }
}