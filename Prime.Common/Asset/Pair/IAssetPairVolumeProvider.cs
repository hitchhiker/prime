using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IAssetPairVolumeProvider : IDescribesAssets
    {
        Task<NetworkPairVolume> GetAssetPairVolume(VolumeContext context);
    }
}