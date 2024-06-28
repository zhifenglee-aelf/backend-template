using System;
using System.Threading.Tasks;
using Grains.Grains;
using Orleans;
using Volo.Abp.DependencyInjection;

namespace Client;

public class HelloService : ITransientDependency
{
    private readonly IClusterClient _clusterClient;
    
    public HelloService(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }
    
    public async Task<string> SayHi()
    {
        IHello friend = _clusterClient.GetGrain<IHello>(0);
        string response = await friend.SayHello("Hi friend!");

        Console.WriteLine($"""
                           {response}

                           Press any key to exit...
                           """);
        
        return "Hi from service";
    }
}