namespace Prime.Base.Messaging.Manager.Models
{
    public class MarketModel
    {
        public MarketModel()
        {
            
        }

        public MarketModel(string id, string code)
        {
            Id = id;
            Code = code;
        }
        
        public string Id { get; set; }
        public string Code { get; set; }
    }
}