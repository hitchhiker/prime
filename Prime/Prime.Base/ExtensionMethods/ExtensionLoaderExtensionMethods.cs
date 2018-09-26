namespace Prime.Core
{
    public static class ExtensionLoaderExtensionMethods 
    {
        public static string ToString(this IExtension extension)
        {
            if (extension is IExtensionPlatform p && p.Platform!= Platform.NotSpecified)
                return extension.Title + " [" + p.Platform + "] " + extension.Version;

            return extension.Title + " " + extension.Version;
        }
    }
}