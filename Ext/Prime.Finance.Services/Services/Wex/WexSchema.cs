using System.Collections.Generic;
using Prime.Finance.Services.Services.Common;

namespace Prime.Finance.Services.Services.Wex
{
    public class WexSchema : CommonSchemaTiLiWe
    {
        #region Private

        public class WithdrawCoinResponse : BaseResponse<WithdrawCoinDataResponse>
        {

        }

        public class WithdrawCoinDataResponse
        {
            /// <summary>
            /// Transaction ID.
            /// </summary>
            public int tId;

            /// <summary>
            /// The amount sent including commission.
            /// </summary>
            public decimal amountSent;

            /// <summary>
            /// Balance after the request.
            /// </summary>
            public Dictionary<string, decimal> funds;
        }

        #endregion

    }
}
