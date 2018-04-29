using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Finance
{
    public interface IAggVolumeDataProvider : IAggregator
    {
        Task<PublicVolumeResponse> GetAggVolumeDataAsync(AggVolumeDataContext context);
    }
}