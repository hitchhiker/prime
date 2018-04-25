using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Prime.KeysManager.Core;
using Prime.KeysManager.Core.Models;
using Prime.KeysManager.Messages;
using Prime.KeysManager.Transport;
using Prime.KeysManager.Utils;

namespace Prime.KeysManager
{
    public class KeysManagerApp
    {
        private readonly ITcpServer _tcpServer;
        private readonly IPrimeService _primeService;

        public short PortNumber { get; set; } = 19991;
        public IPAddress IpAddress { get; set; } = IPAddress.Any;
        
        public KeysManagerApp(ITcpServer tcpServer, IPrimeService primeService)
        {
            _tcpServer = tcpServer;
            _primeService = primeService;
        }
        
        public void Run()
        {
            Subscribe();
            _tcpServer.ExceptionOccurred += TcpServerOnExceptionOccurred;

            // Main app cycle.
            _tcpServer.StartServer(IpAddress, PortNumber);     
        }

        private void Subscribe()
        {
            _tcpServer.Subscribe<ProvidersListMessage>(ProvidersListHandler);
            _tcpServer.Subscribe<PrivateProvidersListMessage>(PrivateProvidersListHandler);
            _tcpServer.Subscribe<ProviderDetailsMessage>(ProviderDetailsHandler);
            _tcpServer.Subscribe<ProviderKeysMessage>(ProviderKeysHandler);
            _tcpServer.Subscribe<DeleteProviderKeysMessage>(DeleteProviderKeysHandler);
            _tcpServer.Subscribe<TestPrivateApiMessage>(TestPrivateApiHandler);
            _tcpServer.Subscribe<GenerateGuidMessage>(FakeClientGuidHandler);
        }

        private void FakeClientGuidHandler(GenerateGuidMessage fakeClientGuidMessage, TcpClient sender)
        {
            Console.WriteLine("Generating client GUID...");

            var guid = Guid.NewGuid();
            _tcpServer.Send(sender, guid);
        }

        private void TestPrivateApiHandler(TestPrivateApiMessage testPrivateApiMessage, TcpClient sender)
        {
            Console.WriteLine("App: Testing private API...");
            var success = true;

            try
            {
                success = _primeService.TestPrivateApi(testPrivateApiMessage.Id, testPrivateApiMessage.Key, testPrivateApiMessage.Secret, testPrivateApiMessage.Extra);
            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine($"App: Error while testing private API: {e.Message}");
            }

            _tcpServer.Send(sender, new OperationResultModel() { Success = success });
        }

        private void DeleteProviderKeysHandler(DeleteProviderKeysMessage deleteProviderKeysMessage, TcpClient sender)
        {
            Console.WriteLine("App: Deleting keys...");
            var success = true;

            try
            {
                _primeService.DeleteKeys(deleteProviderKeysMessage.Id);
            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine($"App: Error while deleting keys: {e.Message}");
            }

            _tcpServer.Send(sender, new OperationResultModel() { Success = success });
        }

        private void ProviderKeysHandler(ProviderKeysMessage providerKeysMessage, TcpClient sender)
        {
            Console.WriteLine("App: Saving keys...");
            var success = true;
            
            try
            {
                _primeService.SaveKeys(providerKeysMessage.Id, providerKeysMessage.Key, providerKeysMessage.Secret, providerKeysMessage.Extra);
            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine($"App: Error while saving keys: {e.Message}");
            }
            
            _tcpServer.Send(sender, new OperationResultModel() { Success = success});
        }

        private void ProviderDetailsHandler(ProviderDetailsMessage providerDetailsMessage, TcpClient sender)
        {
            Console.WriteLine("App: Sending provider details...");

            var providerDetails = _primeService.GetNetworkDetails(providerDetailsMessage.Id);
            _tcpServer.Send(sender, providerDetails);
        }

        private void PrivateProvidersListHandler(PrivateProvidersListMessage privateProvidersListMessage, TcpClient sender)
        {
            Console.WriteLine("App: Private providers list requested... Sending...");

            var providers = _primeService.GetPrivateNetworks();
            _tcpServer.Send(sender, providers);
        }

        private void ProvidersListHandler(ProvidersListMessage providersListMessage, TcpClient sender)
        {
            Console.WriteLine("App: Providers list requested... Sending...");

            var providers = _primeService.GetNetworks();
            _tcpServer.Send(sender, providers);
        }

        private void TcpServerOnExceptionOccurred(object sender, Exception exception)
        {
            Console.WriteLine($"App: Server error occurred: {exception.Message}");
        }

        public void Exit()
        {
            _tcpServer.ShutdownServer();
        }
    }
}