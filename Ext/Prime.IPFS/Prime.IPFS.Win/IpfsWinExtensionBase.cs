using System.Linq.Expressions;
using System.Threading;
using CommandLine;
using Prime.Base;
using Prime.Core;
using Prime.Radiant;

namespace Prime.IPFS
{
    public abstract class IpfsWinExtensionBase : IpfsExtensionBase, IExtensionCommandArgs
    {
        public void InitialiseCommandArgs(PrimeContext context, CommandArgs argsParser)
        {
            argsParser.Add(new CommandArg("ipfs", "IPFS management.", args =>
            {
                void ServerWaitLoop(IpfsWinArguments o)
                {
                    context.M.SendAsync(new IpfsStartRequest());
                    context.L.Log("Prime running in IPFS DAEMON mode.");
                    do
                    {
                        Thread.Sleep(1);
                    } while (!context.IsInstanceStopping);
                    context.L.Log("IPFS DAEMON exiting..");
                }
                Parser.Default.ParseArguments<IpfsWinArguments>(args).WithParsed<IpfsWinArguments>(ServerWaitLoop);
            }));
        }
    }
}