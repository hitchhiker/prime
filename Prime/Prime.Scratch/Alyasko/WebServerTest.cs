using System;

namespace Prime.Bootstrap
{
    /// <summary>
    /// Entry point of Prime.
    /// Starts core. Core starts Radiant.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //var host = new WebHostBuilder().UseUrls("http://localhost:1111").UseKestrel().UseStartup<Startup>().Build();
            //host.Run();
        }

        public class Startup
        {
            /*public void Configure(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Hello World. The Time is: " + DateTime.Now.ToString("hh:mm:ss tt"));
                });
            }*/
        }
    }
}
