using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Prime.Core;

namespace Prime.Base.DStore
{
    public struct ContentUri
    {
        [JsonProperty("proto")]
        public string Protocol { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        public override string ToString()
        {
            return Protocol + "://" + Path;
        }

        public static ContentUri AddFromLocalDirectory(IMessenger m, DirectoryInfo directory)
        {
            var reply = m.SendAndWait<GetContentUriRequest, GetContentUriResponse>(new GetContentUriRequest(directory.FullName));
            return reply?.ContentUri ?? new ContentUri();
        }
    }
}
