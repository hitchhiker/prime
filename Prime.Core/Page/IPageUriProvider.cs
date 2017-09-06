namespace Prime.Core
{
    public interface IPageUriProvider
    {
        bool Disabled { get; }

        GetUriResponse? GetUri(CommandContent commandContent);
    }
}