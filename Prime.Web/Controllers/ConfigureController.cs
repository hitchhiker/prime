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
        [HttpGet("[action]")]
        public async Task ServerTime()
        {
            var response = HttpContext.Response;
            response.Headers.Add("Content-Type", "text/event-stream");

            for (var i = 0; true; ++i)
            {
                await response
                    .WriteAsync($"data: {DateTime.UtcNow:HH:mm:ss tt}\r\r");

                response.Body.Flush();
                await Task.Delay(500);
            }
        }
    }
}
