using MyTemplate.Domain.Grains;
using MyTemplate.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
namespace AbpOrleansAppTemplate.Silo;

[DependsOn(
    typeof(MyTemplateDomainGrainsModule),
    typeof(AbpAspNetCoreSerilogModule),
    //typeof(AbpOrleansAppTemplateMongoDbModule),
    typeof(AbpAutofacModule)
)]
public class MyTemplateOrleansSiloModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<MyTemplateOrleansSiloModule>(); });
        context.Services.AddHostedService<AbpOrleansBackendTemplateHostedService>();
        var configuration = context.Services.GetConfiguration();
        //add dependencies here

        context.Services.AddHttpClient();
    }
}