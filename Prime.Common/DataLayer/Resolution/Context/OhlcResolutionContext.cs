namespace Prime.Common
{
    public class OhlcResolutionContext : OhlcResolutionAdapterContext
    {
        public OhlcResolutionContext() { }

        public OhlcResolutionContext(OhlcResolutionContext ctx) : base(ctx)
        {
            RequestFullDaily = ctx.RequestFullDaily;
        }

        public bool RequestFullDaily { get; set; }
    }
}