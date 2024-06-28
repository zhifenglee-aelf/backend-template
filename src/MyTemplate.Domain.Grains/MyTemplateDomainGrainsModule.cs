using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MyTemplate.Domain.Grains;

[DependsOn(typeof(AbpAutofacModule))]
public class MyTemplateDomainGrainsModule : AbpModule
{
    /*public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<AbpOrleansAppTemplateDomainGrainsModule>(); });
    }*/
}