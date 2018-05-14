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
            new WebSocketTestApp().Run();
        }
    }
}
