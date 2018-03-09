namespace Prime.KeysManager.Messages
{
    public class ProviderKeysMessage
    {
        public string Id { get; set; }
        
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Extra { get; set; }
    }
}