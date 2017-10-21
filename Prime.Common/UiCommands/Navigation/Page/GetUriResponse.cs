namespace Prime.Common
{
    public struct GetUriResponse
    {
        public readonly string PageUrl;
        public readonly bool IsResolved;
        public readonly bool IsTypeFound;

        public GetUriResponse(string pageUrl, bool isResolved, bool isTypeFound)
        {
            PageUrl = pageUrl;
            IsResolved = isResolved;
            IsTypeFound = isTypeFound;
        }

        public static GetUriResponse Empty => new GetUriResponse(null, false, true);

        public static GetUriResponse TypeNotFound => new GetUriResponse(null, false, false);

        public static GetUriResponse FromResolved(string pageUrl)
        {
            return string.IsNullOrWhiteSpace(pageUrl) ? Empty : new GetUriResponse(pageUrl, true, true);
        }

        public static implicit operator string(GetUriResponse instance)
        {
            return instance.PageUrl;
        }

        public static implicit operator GetUriResponse(string instance)
        {
            return GetUriResponse.FromResolved(instance);
        }
    }
}