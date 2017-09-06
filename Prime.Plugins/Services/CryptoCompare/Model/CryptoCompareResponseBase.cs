namespace plugins.Services.CryptoCompare.Model
{
    public class CryptoCompareResponseBase
    {
        public string Response { get; set; }
        public string Message { get; set; }

        public bool IsError() => string.IsNullOrWhiteSpace(Response) || string.Equals(Response, "Error", System.StringComparison.OrdinalIgnoreCase);
    }
}