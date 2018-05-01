using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Prime.IPFS
{
    public interface IApi
    {
        void GetFiles(string hash, DirectoryInfo destination);

        void PinIpfsFile(string hash, bool recursive = true);
    }
}
