using IdentityModel.Client;
using k8s;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace DemoApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> logger;
    private readonly IKubernetes kubernetesClient;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly AppInfo appInfo;

    public DemoController(ILogger<DemoController> logger ,
        IKubernetes kubernetesClient,IHttpClientFactory httpClientFactory, AppInfo appInfo)
    {
        this.logger = logger;
        this.kubernetesClient = kubernetesClient;
        this.httpClientFactory = httpClientFactory;
        this.appInfo = appInfo;
    }
    [HttpGet("allenv")]
    public ActionResult GetAllEnv()
    {
        return Ok(Environment.GetEnvironmentVariables());
    }
    [HttpGet("env")]
    public ActionResult GetEnv([FromQuery] string name)
    {
        return Ok($"{name}={Environment.GetEnvironmentVariable(name)}");
    }
    [HttpGet("refreshconfig")]
    public ActionResult RefreshConfig()
    {
        logger.LogInformation("Received Requesst to Refresh Configuration");

        return Ok("Received Requesst to Refresh Configuration");
    }


    [HttpPost("notify")]
    public ActionResult NotifyConfigChange([FromBody] Dictionary<string,string> payload=default)
    {
        List<string> refreshEndpoints = new List<string>();
        var endpointsSliceList = kubernetesClient.DiscoveryV1.ListNamespacedEndpointSlice("madhu-xplor", labelSelector: "kubernetes.io/service-name in (demo-api-service)");
        foreach (var aEndpointSlice in endpointsSliceList)
        {
            var port = aEndpointSlice.Ports.FirstOrDefault()?.Port;
            foreach (var endpoint in aEndpointSlice.Endpoints)
            {
                var discoverdEndpoint = $"http://{endpoint.Addresses.FirstOrDefault()}:{port}/api/demo/refreshconfig";
                logger.LogInformation("DiscovedEndpoint: {discoverdEndpoint}", discoverdEndpoint);
               var ipOrHostName = endpoint.Addresses.FirstOrDefault();
                if ( ipOrHostName != appInfo.IPAddress)
                {
                    refreshEndpoints.Add(discoverdEndpoint);
                }else
                {
                    logger.LogInformation("Ignoring this app endpoint: {thisAppDiscoverdEndpoint}", discoverdEndpoint);
                }
            }
        }

        logger.LogInformation("Invoking Configuration Refresh : {refresh}", string.Join(",",refreshEndpoints));
        using (var httpClient = httpClientFactory.CreateClient("for_config_refresh"))
        {
            var requests = refreshEndpoints.Select(url =>
            {
                return httpClient.GetAsync(url, HttpContext.RequestAborted);
            });
            Task.WhenAll(requests).Wait(HttpContext.RequestAborted);
        }

        return Ok("Done");

    }
}
