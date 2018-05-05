using System.Linq;

namespace Prime.Core
{
    public class PageUri
    {
        [Bson]
        public string Title { get; set; }

        [Bson]
        public string Uri { get; set; }
    }
}