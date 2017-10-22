using System.Collections.Generic;

namespace Prime.Radiant.Components
{
    public class CloudFlareDnsRecordsResponse : CloudFlareResponseBase
    {
        public List<CloudFlareDnsRecord> result;
    }
}