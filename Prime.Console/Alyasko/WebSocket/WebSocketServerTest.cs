using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime.Core;

namespace Prime.Console.Alyasko.WebSocket
{
    public class WebSocketServerTest
    {
        private readonly ServerContext _sCtx;
        private readonly ClientContext _cCtx;

        private readonly ILogger L;

        public WebSocketServerTest(ServerContext sCtx, ClientContext cCtx)
        {
            _sCtx = sCtx;
            _cCtx = cCtx;

            L = _sCtx.L;
        }

        public void Run()
        {
            var msgSrv = new MessageServer(_sCtx);

            _sCtx.M.RegisterAsync<HelloWsRequest>(this, (x) =>
            {
                _sCtx.L.Log($"HelloWsRequest handled");
                _sCtx.M.Send<HelloWsResponse>(new HelloWsResponse(x));
            });

            _cCtx.M.RegisterAsync<HelloWsResponse>(this, (x) =>
            {
                _cCtx.L.Log($"HelloWsResponse handled");
                _cCtx.L.Log($"Response: {x.Response}");
            });

            // var srcs = _sCtx.Types.Where(x => x.FullName.Contains("Server")).ToList();

            msgSrv.Start();
            L.Log("Message Server started.");

            while (true)
            {

            }

            System.Console.ReadKey();
        }
    }
}
