
namespace Prime.Common
{
    public class ProviderDatas : AssociatedDatasBase<ProviderData>
    {
        private readonly UserContext _context;

        public ProviderDatas(UserContext context)
        {
            _context = context;
        }

        public ProviderData Get(INetworkProvider provider)
        {
            return GetOrCreate(_context, provider);
        }
    }
}