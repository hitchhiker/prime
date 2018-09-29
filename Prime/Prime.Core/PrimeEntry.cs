using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Prime.Base;

namespace Prime.Core
{
    public class PrimeEntry
    {
        private readonly ObjectId _processId = ObjectId.NewObjectId();

        public bool IsStopping { get; set; }

        public bool IsShutdown { get; private set; }

        public void Enter(PrimeBootOptions.Start options)
        {
            Console.WriteLine("Prime: Starting " + _processId);
            Console.WriteLine("Prime: Config location '" + options.ConfigPath + "'");

            var ctx = new PrimeContext(options.ConfigPath);
            var prime = new PrimeInstance(ctx);
            prime.Start();
            Console.WriteLine("Prime: Started " + _processId);
            do
            {
                Thread.Sleep(1);
            } while (!IsStopping);

            Console.WriteLine("Prime: Shutting down " + _processId);
            prime.Stop();
            IsShutdown = true;
            Console.WriteLine("Prime: Stopped " + _processId);
        }
    }
}
