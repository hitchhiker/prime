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

        public static ContentUri Parse(string parse)
        {
            if (string.IsNullOrWhiteSpace(parse))
                throw new ArgumentException(nameof(parse) + " is empty in " + nameof(ContentUri) + ".");

            if (!parse.Contains("://"))
                throw new Exception(parse + $" is not a valid {nameof(ContentUri)}");

            if (parse.StartsWith("ipns://"))
                return new ContentUri() {Protocol = "ipns", Path = parse.Substring(7)};

            if (parse.StartsWith("ipfs://"))
                return new ContentUri() { Protocol = "ipfs", Path = parse.Substring(7) };

            throw new Exception($"Unable to parse '{parse}' to a {nameof(ContentUri)}");
        }
    }
}
