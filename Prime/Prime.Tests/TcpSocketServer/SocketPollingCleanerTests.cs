using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Prime.Base;
using Prime.SocketServer.Transport;
using Prime.SocketServer.Transport.Cleaner;
using Xunit;

namespace Prime.Tests.TcpSocketServer
{
    public class SocketPollingCleanerTests
    {
        [Fact]
        public void TestUpdating()
        {
            var cleaner = new SocketPollingCleaner<ObjectId>(objectId => false);
            var id = ObjectId.NewObjectId();
            cleaner.UpdateActivity(id);

            var lastTime = cleaner.GetLastUpdateTime(id);
            Thread.Sleep(5000);
            lastTime = cleaner.GetLastUpdateTime(id);
        }

    }
}
