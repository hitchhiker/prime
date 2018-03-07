using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Prime.KeysManager.Core;
using Prime.KeysManager.Messages;
using Prime.KeysManager.Transport;

namespace Prime.KeysManager
{
    public class KeysManagerApp
    {
        private ITcpServer _tcpServer;
        private IPrimeService _primeService;
        
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
            _tcpServer.CreateServer(IPAddress.Parse("127.0.0.1"), 8082);     
        }

        private void Subscribe()
        {
            _tcpServer.Subscribe<ProvidersListMessage>(ProvidersListHandler);
            _tcpServer.Subscribe<PrivateProvidersListMessage>(PrivateProvidersListHandler);
            _tcpServer.Subscribe<ProviderDetailsMessage>(ProviderDetailsHandler);
        }

        private void ProviderDetailsHandler(ProviderDetailsMessage providerDetailsMessage)
        {
            Console.WriteLine("Sending provider details...");

            var providerDetails = _primeService.GetProviderDetails(providerDetailsMessage.Id);
            _tcpServer.Send(providerDetails);
        }

        private void PrivateProvidersListHandler(PrivateProvidersListMessage privateProvidersListMessage)
        {
            Console.WriteLine("Private providers list requested... Sending...");

            var providers = _primeService.GetPrivateNetworks();
            _tcpServer.Send(providers);
        }

        private void ProvidersListHandler(ProvidersListMessage providersListMessage)
        {
            Console.WriteLine("Providers list requested... Sending...");

            var providers = _primeService.GetNetworks();
            _tcpServer.Send(providers);
        }

        private void TcpServerOnExceptionOccurred(object sender, Exception exception)
        {
            Console.WriteLine($"Error: {exception.Message}");
        }
    }
}