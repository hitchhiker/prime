using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IAggVolumeDataProvider : INetworkProvider
    {
        Task<VolumeDataExchanges> GetAggVolumeDataAsync(AggVolumeDataContext context);
    }
}