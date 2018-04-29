using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IPublicVolumeProvider : IDescribesAssets
    {
        Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context);

        VolumeFeatures VolumeFeatures { get; }
    }
}