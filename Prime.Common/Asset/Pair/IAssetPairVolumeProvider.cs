using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicVolumeProvider : IDescribesAssets
    {
        Task<NetworkPairVolume> GetPublicVolumeAsync(VolumeContext context);
    }
}