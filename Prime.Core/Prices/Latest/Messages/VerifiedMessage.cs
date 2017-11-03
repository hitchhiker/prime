namespace Prime.Core.Prices.Latest.Messages
{
    internal class VerifiedMessage
    {
        internal readonly Request Request;

        internal VerifiedMessage(Request request)
        {
            Request = request;
        }
    }
}