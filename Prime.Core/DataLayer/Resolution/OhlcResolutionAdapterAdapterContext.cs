namespace Prime.Core
{
    public class OhlcResolutionAdapterAdapterContext : OhlcResolutionAdapterContext
    {
        public OhlcResolutionAdapterAdapterContext() { }

        public OhlcResolutionAdapterAdapterContext(OhlcResolutionAdapterAdapterContext ctx) : base(ctx)
        {
            RequestFullDaily = ctx.RequestFullDaily;
        }

        public bool RequestFullDaily { get; set; }
    }
}