using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Prime.Web.Controllers
{
    [Route("/api/[controller]")]
    public class ConfigureController : Controller
    {
        private bool _isTimeUtc;

        public ConfigureController()
        {
            TcpBridge.TimeKindUpdated += message => { _isTimeUtc = message.IsUtc; };
        }
        
        [HttpGet("[action]")]
        public async Task ServerTime()
        {
            var response = HttpContext.Response;
            response.Headers.Add("Content-Type", "text/event-stream");

            while(true)
            {
                await response
                    .WriteAsync($"data: {(_isTimeUtc ? DateTime.UtcNow: DateTime.Now):HH:mm:ss tt}\r\r");

                response.Body.Flush();
                await Task.Delay(500);
            }
        }
    }
}
