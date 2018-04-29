using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IAggVolumeDataProvider : IAggregator
    {
        Task<PublicVolumeResponse> GetAggVolumeDataAsync(AggVolumeDataContext context);
    }
}