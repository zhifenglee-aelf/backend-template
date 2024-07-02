using System;
using System.Threading.Tasks;
using Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyTemplate.Domain.Grains.Grains;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers.MongoDB.Configuration;
using Serilog;
using Serilog.Events;

[assembly: GenerateCodeForDeclaringAssembly(typeof(IHello))]

namespace MyTemplate;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting HttpApi.Host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host
                .UseOrleansClient((context, clientBuilder) =>
                {
                    var config = context.Configuration;
                    
                    clientBuilder
                        .UseMongoDBClient(config["Orleans:MongoDBClient"])
                        .UseMongoDBClustering(options =>
                        {
                            options.DatabaseName = config["Orleans:DataBase"];
                            options.Strategy = MongoDBMembershipStrategy.SingleDocument;
                        })
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = config["Orleans:ClusterId"];
                            options.ServiceId = config["Orleans:ServiceId"];
                        });
                })
                .ConfigureDefaults(args)
                .UseAutofac()
                .UseSerilog();
            await builder.AddApplicationAsync<MyTemplateHttpApiHostModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            
            //TODO: can be removed along with HelloService.cs after setting up proper grains
            app.MapGet("/hi", async ([FromServices] HelloService helloService) =>
            {
                return await helloService.SayHi();
            });
            
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
