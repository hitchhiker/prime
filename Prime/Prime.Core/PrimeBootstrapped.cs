using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Prime.Base;

namespace Prime.Core
{
    public class PrimeBootstrapped
    {
        private readonly ObjectId _processId = ObjectId.NewObjectId();

        public bool IsStopping { get; set; }

        public bool IsShutdown { get; private set; }

        public void Enter(string[] args)
        {
            Console.WriteLine("Prime Bootstrapper: Starting " + _processId);
            var ctx = PrimeContext.ForDevelopmentClient();
            var prime = new PrimeInstance(ctx);
            prime.Start();
            prime.C.M.Send(new IpfsStartRequest());
            Console.WriteLine("Prime Bootstrapper: Started " + _processId);
            do
            {
                Thread.Sleep(1);
            } while (!IsStopping);

            Console.WriteLine("Prime Bootstrapper: Shutting down " + _processId);
            prime.Stop();
            IsShutdown = true;
            Console.WriteLine("Prime Bootstrapper: Stopped " + _processId);
        }
    }
}
