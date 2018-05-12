using System;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.KeysManager.Core;
using Prime.KeysManager.Messages;

namespace Prime.KeysManager
{
    public class KeyManagerServer
    {
        private readonly ServerContext _context;
        private readonly IApiKeyService _apiKeyService;

        public readonly ILogger L;
        public readonly IMessenger M;
        
        public KeyManagerServer(ServerContext context)
        {
            _context = context;
            _apiKeyService = new ApiKeyService(context);
            L = _context.L;
            M = _context.M;
        }

        private void Log(string text)
        {
            L.Log($"({typeof(KeyManagerServer).Name}) : {text}");
        }
        
        public void Run()
        {
            M.RegisterAsync<ProvidersListRequestMessage>(this, ProvidersListHandler);
            M.RegisterAsync<PrivateProvidersListRequestMessage>(this, PrivateProvidersListHandler);
            M.RegisterAsync<ProviderDetailsRequestMessage>(this, ProviderDetailsHandler);
            M.RegisterAsync<ProviderKeysRequestMessage>(this, ProviderKeysHandler);
            M.RegisterAsync<DeleteProviderKeysRequestMessage>(this, DeleteProviderKeysHandler);
            M.RegisterAsync<TestPrivateApiRequestMessage>(this, TestPrivateApiHandler);
            M.RegisterAsync<GenerateGuidRequestMessage>(this, FakeClientGuidHandler);
        }

        private void FakeClientGuidHandler(GenerateGuidRequestMessage request)
        {
            Log("Generating client GUID...");

            var guid = Guid.NewGuid();
            M.SendAsync(new GenerateGuidResponseMessage(request, guid));
        }

        private void TestPrivateApiHandler(TestPrivateApiRequestMessage request)
        {
            Log("Testing private API...");
            var success = true;

            try
            {
                success = _apiKeyService.TestPrivateApi(request.Id, request.Key, request.Secret, request.Extra);
            }
            catch (Exception e)
            {
                success = false;
                Log($"Error while testing private API: {e.Message}");
            }

            M.SendAsync(new TestPrivateApiResponseMessage(request, success));
        }

        private void DeleteProviderKeysHandler(DeleteProviderKeysRequestMessage request)
        {
            Log("Deleting keys...");
            var success = true;

            try
            {
                _apiKeyService.DeleteKeys(request.Id);
            }
            catch (Exception e)
            {
                success = false;
                Log($"Error while deleting keys: {e.Message}");
            }

            M.SendAsync(new DeleteProviderKeysResponseMessage(request, success));
        }

        private void ProviderKeysHandler(ProviderKeysRequestMessage request)
        {
            Log("Saving keys...");
            var success = true;
            
            try
            {
                _apiKeyService.SaveKeys(request.Id, request.Key, request.Secret, request.Extra);
            }
            catch (Exception e)
            {
                success = false;
                Log($"Error while saving keys: {e.Message}");
            }

            M.SendAsync(new ProviderKeysResponseMessage(request, success));
        }

        private void ProviderDetailsHandler(ProviderDetailsRequestMessage request)
        {
            Log("Sending provider details...");

            var response = _apiKeyService.GetNetworkDetails(request.Id);
            M.SendAsync(new ProviderDetailsResponseMessage(request, response));
        }

        private void PrivateProvidersListHandler(PrivateProvidersListRequestMessage request)
        {
            Log("Private providers list requested... Sending...");

            var response = _apiKeyService.GetPrivateNetworks().AsList();
            M.SendAsync(new PrivateProvidersListResponseMessage(request, response));
        }

        private void ProvidersListHandler(ProvidersListRequestMessage request)
        {
            Log("Providers list requested... Sending...");

            var response = _apiKeyService.GetNetworks().AsList();
            M.SendAsync(new ProvidersListResponseMessage(request, response));
        }

        public void Stop()
        {
            M.UnregisterAsync(this);
        }
    }
}