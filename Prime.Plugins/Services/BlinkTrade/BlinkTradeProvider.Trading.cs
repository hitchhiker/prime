using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Plugins.Services.BlinkTrade
{
    public partial class BlinkTradeProvider /*: IOrderLimitProvider, IWithdrawalPlacementProvider*/
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            if (!rawResponse.ResponseMessage.IsSuccessStatusCode && rawResponse.TryGetContent(out BlinkTradeSchema.ErrorResponse baseResponse))
            {
                throw new ApiResponseException(
                    $"Error: {baseResponse.Status} - {baseResponse.Description.TrimEnd('.')}", this, method);
            }
        }
    }
}
