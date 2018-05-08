using System.Threading.Tasks;

namespace Prime.Finance
{
    public interface IPublicVolumeProvider : IDescribesAssets
    {
        Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context);

        VolumeFeatures VolumeFeatures { get; }
    }
}