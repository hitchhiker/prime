namespace Prime.Common
{
    public abstract class ResponseModelBase : ModelBase
    {
        public int ApiHitsCount { get; set; } = 1;

        public void ApiHit()
        {
            ApiHitsCount++;
        }
    }
}