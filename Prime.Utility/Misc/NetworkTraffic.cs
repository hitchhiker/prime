using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

/*
 * https://stackoverflow.com/a/442459/1318333
<configuration>
  <system.net>
    <settings>
      <performanceCounters enabled="true" />
    </settings>
  </system.net>
</configuration>

*/

// https://pastebin.com/f371375d6
public class NetworkTraffic
{
  private PerformanceCounter bytesSentPerformanceCounter;
  private PerformanceCounter bytesReceivedPerformanceCounter;
 
  public NetworkTraffic()
  {
    bytesSentPerformanceCounter = new PerformanceCounter();
    bytesSentPerformanceCounter.CategoryName = ".NET CLR Networking";
    bytesSentPerformanceCounter.CounterName = "Bytes Sent";
    bytesSentPerformanceCounter.InstanceName = GetInstanceName();
    bytesSentPerformanceCounter.ReadOnly = true;
 
    bytesReceivedPerformanceCounter = new PerformanceCounter();
    bytesReceivedPerformanceCounter.CategoryName = ".NET CLR Networking";
    bytesReceivedPerformanceCounter.CounterName = "Bytes Received";
    bytesReceivedPerformanceCounter.InstanceName = GetInstanceName();
    bytesReceivedPerformanceCounter.ReadOnly = true;
  }
 
 
  public float GetBytesSent()
  {
    float bytesSent = bytesSentPerformanceCounter.RawValue;
 
    return bytesSent;
  }
 
  public float GetBytesReceived()
  {
    float bytesReceived = bytesReceivedPerformanceCounter.RawValue;
 
    return bytesReceived;
  }
 
  private static string GetInstanceName()
  {
    // Used Reflector to find the correct formatting:
    string assemblyName = GetAssemblyName();
    if ((assemblyName == null) || (assemblyName.Length == 0))
    {
      assemblyName = AppDomain.CurrentDomain.FriendlyName;
    }
    StringBuilder builder = new StringBuilder(assemblyName);
    for (int i = 0; i < builder.Length; i++)
    {
      switch (builder[i])
      {
        case '/':
        case '\\':
        case '#':
          builder[i] = '_';
          break;
        case '(':
          builder[i] = '[';
          break;
 
        case ')':
          builder[i] = ']';
          break;
      }
    }
 
    return string.Format(CultureInfo.CurrentCulture,
                         "{0}[{1}]",
                         builder.ToString(),
                         Process.GetCurrentProcess().Id);
  }
  private static string GetAssemblyName()
  {
    string str = null;
    Assembly entryAssembly = Assembly.GetEntryAssembly();
    if (entryAssembly != null)
    {
      AssemblyName name = entryAssembly.GetName();
      if (name != null)
      {
        str = name.Name;
      }
    }
    return str;
  }
 
 
  public static void Main()
  {
    NetworkTraffic networkTraffic = new NetworkTraffic();
    try
    {
      while (true)
      {
        WebRequest webRequest = WebRequest.Create("http://www.google.com");
        webRequest.Method = "GET";
 
        using (WebResponse response = webRequest.GetResponse())
        using (Stream responseStream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(responseStream))
        {
        }
        Console.WriteLine("Bytes sent: {0}", networkTraffic.GetBytesSent());
        Console.WriteLine("Bytes received: {0}", networkTraffic.GetBytesReceived());
        Thread.Sleep(1000);
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
    Console.ReadLine();
  }
}