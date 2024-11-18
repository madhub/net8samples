using System.Net.Sockets;
using System.Net;

namespace DemoApi;

public class AppInfo
{
    public String AppName { get; set; } = "DemoApp";
    public String IPAddress { get;}
    public AppInfo()
    {
        var name = Dns.GetHostName(); // get container id
        var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        IPAddress = ip.ToString();

    }
}
