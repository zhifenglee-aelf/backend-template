using MyTemplate.Domain.Grains.Grains;
using Orleans;

[assembly: GenerateCodeForDeclaringAssembly(typeof(IHello))]
//add more grain interfaces below this line

namespace MyTemplate.Grains;