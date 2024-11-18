// See https://aka.ms/new-console-template for more information
using System.IO.Pipes;
using System.Net.Http.Json;
using System.Net.Sockets;

HttpClient client = CreateHttpClientConnectionToDockerEngine();
String dockerUrl = "http://localhost/v1.41/containers/json";
var containers = client.GetFromJsonAsync<List<Container>>(dockerUrl).GetAwaiter().GetResult();
Console.WriteLine("Container List:...");
foreach (var item in containers)
{
    Console.WriteLine(item);
}


// Create HttpClient to Docker Engine using NamedPipe & UnixDomain
HttpClient CreateHttpClientConnectionToDockerEngine()
{
    SocketsHttpHandler socketsHttpHandler =
        OperatingSystem.IsWindows() switch
        {
            true => GetSocketHandlerForNamedPipe(),
            false => GetSocketHandlerForUnixSocket(),
        };
    return new HttpClient(socketsHttpHandler);

    // Local function to create Handler using NamedPipe
    static SocketsHttpHandler GetSocketHandlerForNamedPipe()
    {
        Console.WriteLine("Connecting to Docker Engine using Named Pipe:");
        SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler();
        // Custom connection callback that connects to NamedPiper server
        socketsHttpHandler.ConnectCallback = async (sockHttpConnContext, ctxToken) =>
        {
            Uri dockerEngineUri = new Uri("npipe://./pipe/docker_engine");
            NamedPipeClientStream pipeClientStream = new NamedPipeClientStream(dockerEngineUri.Host,
                                                    dockerEngineUri.Segments[2],
                                                    PipeDirection.InOut, PipeOptions.Asynchronous);
            await pipeClientStream.ConnectAsync(ctxToken);
            return pipeClientStream;
        };
        return socketsHttpHandler;
    }
    // Local function to create Handler using Unix Socket
    static SocketsHttpHandler GetSocketHandlerForUnixSocket()
    {
        Console.WriteLine("Connecting to Docker Engine using Unix Domain Socket:");
        SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler();
        // Custom connection callback that connects to Unixdomain Socket
        socketsHttpHandler.ConnectCallback = async (sockHttpConnContext, ctxToken) =>
        {
            Uri dockerEngineUri = new Uri("unix:///var/run/docker.sock");
            var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);

            var endpoint = new UnixDomainSocketEndPoint(dockerEngineUri.AbsolutePath);
            await socket.ConnectAsync(endpoint, ctxToken);
            return new NetworkStream(socket);
        };
        return socketsHttpHandler;
    }
}

/// <summary>
/// Record class to hold the container information
/// </summary>
/// <param name="Names"></param>
/// <param name="Image"></param>
/// <param name="ImageID"></param>
/// <param name="Command"></param>
/// <param name="State"></param>
/// <param name="Status"></param>
/// <param name="Created"></param>
public record Container(List<String> Names, String Image, String ImageID,
                        String Command, String State, String Status, 
                        int Created);

