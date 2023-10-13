// See https://aka.ms/new-console-template for more information


// Default Docker Engine on Windows
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerStats;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

DockerClient client = default;

if ( RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    client = new DockerClientConfiguration(
        new Uri("npipe://./pipe/docker_engine"))
         .CreateClient();
}else { 
// Default Docker Engine on Linux

client = new DockerClientConfiguration(
    new Uri("unix:///var/run/docker.sock"))
     .CreateClient();
}

Console.WriteLine($"ContainerName,CpuPercent,MemUsageInKB,MaxAviMemInKB,MemUsageInPercent,DateTime");
while (true)
{
    var containers = client.Containers.ListContainersAsync(new ContainersListParameters()
    {
        All = true,
        Filters = new Dictionary<string, IDictionary<string, bool>>
        {
            ["status"] = new Dictionary<string, bool>
            {
                ["running"] = true,
                ["created"] = true
            }
        },

    }).GetAwaiter().GetResult();
    foreach (var container in containers)
    {
        //Console.WriteLine($"Name:{string.Join(",",container.Names)} container.State: {container.State}");
        //if (container.State != "running" || container.State != "created")
        //{
        //    continue;
        //}
        try
        {
            client.Containers.GetContainerStatsAsync(container.ID, new ContainerStatsParameters() { OneShot = true, Stream = false }, new Progress<ContainerStatsResponse>(m =>
            {
                //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                //{

                //}
                //else
                {
                    // https://github.com/docker/cli/blob/fc247d6911944b3c8dca524c93f291e5f79ec3da/cli/command/container/stats_helpers.go#L166
                    var previousCPU = m.PreCPUStats.CPUUsage.TotalUsage;
                    var previousSystem = m.PreCPUStats.SystemUsage;
                    var cpuPercent = 0.0;
                    float cpuDelta = (float)m.CPUStats.CPUUsage.TotalUsage - (float)previousCPU;
                    float systemDelta = (float)(m.CPUStats.SystemUsage) - (float)previousSystem;
                    float onlineCPUs = (float)(m.CPUStats.OnlineCPUs);
                    if (onlineCPUs == 0.0)
                    {
                        onlineCPUs = (float)m.CPUStats.CPUUsage.PercpuUsage.Count();
                    }


                    if (systemDelta > 0.0 && cpuDelta > 0.0)
                    {
                        cpuPercent = (cpuDelta / systemDelta) * onlineCPUs * 100.0;
                    }
                    float memUsageInPercent = ((float)m.MemoryStats.Usage / (float)m.MemoryStats.Limit) * (float)100.0;

                    //Console.WriteLine($"Name:{m.Name},CpuPercent:{cpuPercent:F2} %, MemUsage/Limit:{Bytes.GetReadableSize((long)m.MemoryStats.Usage)}/{Bytes.GetReadableSize((long)m.MemoryStats.Limit)},MemPercentage:{memUsageInPercent:F2} ");
                    Console.WriteLine($"{m.Name},{cpuPercent:F2},{Bytes.ToKilobytes((long)m.MemoryStats.Usage)},{Bytes.ToKilobytes((long)m.MemoryStats.Limit)},{memUsageInPercent:F2},{DateTime.Now.ToString("o", CultureInfo.InvariantCulture)}");

                }
            })).GetAwaiter().GetResult();
        }
        catch (Exception exp)
        {
            
        }
        

    }
    Thread.Sleep(2000);
}
