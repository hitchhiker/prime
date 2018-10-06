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

            prime.Start();

            BootstrapperEntry.EnterSecondary(prime, args.Split(' '));
;        }
    }
}