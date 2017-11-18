using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IAggVolumeDataProvider : INetworkProvider
    {
        Task<NetworkPairVolumeData> GetAggVolumeDataAsync(AggVolumeDataContext context);
    }
}