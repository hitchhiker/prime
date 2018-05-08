using System.Collections.Generic;

namespace CryptoFacilities.Api.V1
{
    internal class GetContractsResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }

        public List<Contract> Contracts { get; set; }
    }
}