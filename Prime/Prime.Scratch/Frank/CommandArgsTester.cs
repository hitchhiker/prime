using System;
using Prime.Core;

namespace Prime.Scratch
{
    public static class CommandArgsTester
    {
        public static void Go(PrimeInstance prime, string args)
        {
            prime.L.Log("ArgsTester with: " + args);

            BootstrapperEntry.WriteEntranceHeader(prime);

            var configPath = prime.C.Config.ConfigLoadedFrom.FullName;

            prime.Start();

            BootstrapperEntry.EnterSecondary(prime, args.Split(' '));
;        }
    }
}