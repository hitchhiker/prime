#region

using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

#endregion

namespace Prime.Radiant.Components
{
    public class CloudFlare
    {
        private readonly PublishManagerContext _context;
        private readonly string _apiEndpoint = "https://api.cloudflare.com/client/v4";

        public CloudFlare(PublishManagerContext context)
        {
            _context = context;
        }

        public CloudFlareDnsRecordsResponse GetDnsRecords(string zoneId)
        {
            var request = WebRequest.CreateHttp(_apiEndpoint + "/zones/" + zoneId + "/dns_records");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("X-Auth-Email", _context.CloudFlareEmail);
            request.Headers.Add("X-Auth-Key", _context.CloudFlareApiGlobal);

            var responseContent = "";

            using (var response = request.GetResponse())
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                    responseContent = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<CloudFlareDnsRecordsResponse>(responseContent);
        }

        public CloudFlareDnsUpdateResponse UpdateDnsRecord(string zoneId, string recordId, string type, string name, string content, int ttl = 1, bool proxied = false)
        {
            var request = WebRequest.CreateHttp(_apiEndpoint + "/zones/" + zoneId + "/dns_records/" + recordId);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Headers.Add("X-Auth-Email", _context.CloudFlareEmail);
            request.Headers.Add("X-Auth-Key", _context.CloudFlareApiGlobal);

            var json = new JavaScriptSerializer().Serialize(new
            {
                type,
                name,
                content,
                ttl,
                proxied = false
            });
            

            using (var outStream = new StreamWriter(request.GetRequestStream()))
                outStream.Write(json);
            
            var responseContent = "";

            using (var response = request.GetResponse())
            using (var streamReader = new StreamReader(response.GetResponseStream()))
                responseContent = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<CloudFlareDnsUpdateResponse>(responseContent);
        }
    }
}
