using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.IPFS;
using Prime.IPFS.Win64;

namespace IPFS.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dir = new DirectoryInfo("c://tmp//ipfs-ext");
            var ipfs = new IpfsInstance(new IpfsInstanceContext(dir, new IpfsWin64()));
            ipfs.Daemon.Start();
        }
    }
}
