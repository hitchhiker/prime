namespace Prime.Common
{
    public class ApiConfiguration
    {
        public bool HasKey { get; set; } = true;
        
        public bool HasSecret { get; set; }

        public bool HasExtra { get; set; }

        public string ApiKeyName { get; set; } = "Api Key";

        public string ApiSecretName { get; set; } = "Api Secret";

        public string ApiExtraName { get; set; }

        public string InformationUri { get; set; }

        public static ApiConfiguration KeyOnly => new ApiConfiguration();

        public static ApiConfiguration Standard2 => new ApiConfiguration() {HasSecret = true};

        public static ApiConfiguration Standard3 => new ApiConfiguration() {HasSecret = true, HasExtra = true};

        public static ApiConfiguration None => new ApiConfiguration() {HasKey = false};
    }
}