using System;
using System.Linq;

namespace Prime.Common
{
    public abstract class CommandContent : CommandBase, IEquatable<CommandContent>
    {
        private string _title;

        [Bson]
        public string Title
        {
            get => _title ?? DefaultTitle;
            set => _title = value;
        }

        public abstract string DefaultTitle { get; }

        public GetUriResponse GetUri()
        {
            return PageUris.I.Providers.Select(x => x.GetUri(this)).FirstOrDefault(x => x != null) ?? GetUriResponse.TypeNotFound;
        }

        public bool Equals(CommandContent other)
        {
            return base.Equals(other);
        }
    }
}