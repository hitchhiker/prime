using System;
using LiteDB;

namespace Prime.Core
{
    public class PackageMeta : IExtension
    {
        public PackageMeta() { }

        public PackageMeta(IExtension ext)
        {
            Title = ext.Title;
            Id = ext.Id;
            Platform = ext.Platform;
            Version = ext.Version;
        }

        public string Title { get; set; }

        public ObjectId Id { get; set; }

        public Platform Platform { get; set; }

        public Version Version { get; set; }

        public string ToJsonSimple()
        {
            return $"{{\"title\":\"{Title}\",{Environment.NewLine}\"id\":\"{Id}\",{Environment.NewLine}\"platform\":\"{Platform}\",{Environment.NewLine}\"version\":\"{Version}\"}}";
        }
    }
}