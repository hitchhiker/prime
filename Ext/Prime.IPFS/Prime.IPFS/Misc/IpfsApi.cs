using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ipfs.Api;
using Nito.AsyncEx;
using Prime.Core;

namespace Prime.IPFS
{
    public class IpfsApi
    {
        private readonly IpfsInstance _instance;

        public ILogger L => _instance.L;

        private IpfsClient Client => _instance.Client;

        public IpfsApi(IpfsInstance instance)
        {
            _instance = instance;
        }

        public void GetFiles(string hash, DirectoryInfo destination)
        {
            L.Info("Requesting data stream from IPFS (" + hash + ")");

            using (var f = AsyncContext.Run(() => Client.FileSystem.ReadFileAsync(hash)))
            {
               //
            }
        }

        public void PinFile(string hash, bool recursive = true)
        {
            var pinned = AsyncContext.Run(() => Client.Pin.AddAsync(hash, recursive));

            if (pinned != null)
                return;

            L.Error("Node could not be pinned.");
        }
    }
}
