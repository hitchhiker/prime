using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicVolumeProvider : IDescribesAssets
    {
        Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context);

        VolumeFeatures VolumeFeatures { get; }
    }
}