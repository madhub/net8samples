// See https://aka.ms/new-console-template for more information
using k8s;
using k8s.Models;

Console.WriteLine("Hello, World!");


// Replace these with your actual values
string serviceNamespace = "madhu-xplor";
string labelKey = "your-label-key";
string labelValue = "your-label-value";

// Kubernetes client configuration
KubernetesClientConfiguration config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

// Create Kubernetes client
IKubernetes client = new Kubernetes(config);

// Filter pods by label (optional)
// Get service endpoints

var endpointsSliceList = client.DiscoveryV1.ListNamespacedEndpointSlice(serviceNamespace,labelSelector: "kubernetes.io/service-name in (demo-api-service)") ;
foreach (var aEndpointSlice in endpointsSliceList)
{
    var port = aEndpointSlice.Ports.FirstOrDefault()?.Port;
    foreach(var endpoint in aEndpointSlice.Endpoints)
    {
        Console.WriteLine($"http://{endpoint.Addresses.FirstOrDefault()}:{port}");
    }
}

var eps = client.CoreV1.ListNamespacedService(serviceNamespace);//, null, null,null, "name=http-echo");



//var endpoints2 = await client.CoreV1.ListNamespacedEndpointsAsync(serviceNamespace);
//Console.WriteLine(endpoints2);

var endpoints = client.CoreV1.ListNamespacedEndpoints(serviceNamespace, labelSelector: "kubernetes.io/service-name in (demo-api-service)");
Console.WriteLine(endpoints);