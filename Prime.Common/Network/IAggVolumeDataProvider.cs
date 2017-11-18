using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IAggVolumeDataProvider : IAggregator
    {
        Task<NetworkPairVolumeData> GetAggVolumeDataAsync(AggVolumeDataContext context);
    }
}