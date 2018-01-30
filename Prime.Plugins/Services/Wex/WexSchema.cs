using System;
using System.Collections.Generic;
using System.Text;
using Prime.Plugins.Services.Common;

namespace Prime.Plugins.Services.Wex
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
