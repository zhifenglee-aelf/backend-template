using System;
using System.Threading.Tasks;
using Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyTemplate.Extensions;
using Serilog;
using Serilog.Events;

namespace MyTemplate;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        ConfigureLogger();

        try
        {
            Log.Information("Starting HttpApi.Host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host
                .UseOrleansClientConfiguration()
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
    
    private static void ConfigureLogger()
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
    }
}
