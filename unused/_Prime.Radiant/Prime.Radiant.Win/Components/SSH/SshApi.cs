using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Prime.Radiant.Components.SSH
{
    public class SshApi
    {
        private readonly PublishManagerContext _context;

        public SshApi(PublishManagerContext context)
        {
            _context = context;
        }

        public void SendCommands(List<string> commands, Action<SshCommand> afterExecuted = null)
        {
            _context.L.Info($"Connecting via SSH to {_context.SshUsername}@{_context.SshUri}");
            _context.L.Info(" ");
            var privateKeyFile = new PrivateKeyFile(_context.SshPrivateKeyPath);
            var connectionInfo = new ConnectionInfo(_context.SshUri, _context.SshUsername, new PrivateKeyAuthenticationMethod(_context.SshUsername, privateKeyFile));
            using (var client = new SshClient(connectionInfo))
            {
                client.Connect();
                foreach (var c in commands)
                {
                    _context.L.Info($"{_context.SshUsername}@ssh :~$ {c}");
                    var result = client.RunCommand(c);
                    afterExecuted?.Invoke(result);
                }
            }
            _context.L.Info(" ");
        }
    }
}
