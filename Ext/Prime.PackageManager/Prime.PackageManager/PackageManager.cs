namespace Prime.PackageManager
{
    public class PackageManager
    {
        private readonly PackageManagerContext _context;

        public PackageManager(PackageManagerContext context)
        {
            _context = context;
        }

        public void Process()
        {
            var cbuild = new CatalogueBuilder(_context);
            cbuild.Build();
        }
    }
}