using System.Threading.Tasks;
using RestEase;

namespace plugins
{
    public interface ICryptoFacilitiesApi
    {
        // The [Get] attribute marks this method as a GET request
        // The "users" is a relative path the a base URL, which we'll provide later
        // "{userId}" is a placeholder in the URL: the value from the "userId" method parameter is used
        [Get("history?symbol={symbol}")]
        Task<Response<HistoryResponse>> GetHistoryAsync([Path] string symbol);
    }
}