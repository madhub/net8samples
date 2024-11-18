using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Net8Samples;
internal class Class1
{
    static void Main2(string[] args)
    {
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

// create HttpClient with SocketsHttpHandler
var httpClient  = new HttpClient(socketsHttpHandler);
// make Http Request .
        
    }
}
