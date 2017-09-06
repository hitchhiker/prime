using RestSharp;

namespace Rokolab.BitstampClient
{
    public interface IRequestAuthenticator
    {
        void Authenticate(RestRequest request);
    }
}