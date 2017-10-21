namespace Prime.Common
{
    public interface IPageUriProvider
    {
        bool Disabled { get; }

        GetUriResponse? GetUri(CommandContent commandContent);
    }
}