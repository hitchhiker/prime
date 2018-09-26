using System.IO;
using System.Threading.Tasks;
using Ipfs.CoreApi;
using Prime.Base.DStore;
using Prime.Core;

namespace Prime.IPFS.Messaging
{
    public class ContentUriHelper
    {
        private readonly IpfsMessenger _ipfsM;

        public ContentUriHelper(IpfsMessenger ipfsM)
        {
            _ipfsM = ipfsM;
        }

        internal void GetContentUriRequest(GetContentUriRequest m)
        {
            if (m.Protocol != "ipfs")
                return;
            
            _ipfsM.IpfsInstance.StartAndDo(async delegate
            {
                var ci = await GetContentUri(m);

                var response = new GetContentUriResponse(m, "ipfs");

                if (ci.Path != null)
                {
                    response.Success = true;
                    response.ContentUri = ci;
                }

                _ipfsM.M.Send(response);
            });
        }

        private async Task<ContentUri> GetContentUri(GetContentUriRequest m)
        {
            var c = _ipfsM.IpfsInstance.Client;
            var type = m.LocalPath.DetectFileSystemPath();

            if (type==0)
                return new ContentUri();

            var node = type == 2
                ? await c.FileSystem.AddDirectoryAsync(m.LocalPath)
                : await c.FileSystem.AddFileAsync(m.LocalPath);

            return node==null ? new ContentUri() : new ContentUri() {Protocol = "ipfs", Path = node.Id.Hash.ToBase58()};
        }
    }
}