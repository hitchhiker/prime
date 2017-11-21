using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IAggVolumeDataProvider : IAggregator
    {
        Task<PublicVolumeResponse> GetAggVolumeDataAsync(AggVolumeDataContext context);
    }
}