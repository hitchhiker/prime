using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IAssetPairVolumeProvider : IDescribesAssets
    {
        Task<VolumeResult> GetVolumeAsync(VolumeContext context);
    }
}