
using Microsoft.Extensions.DependencyInjection;

namespace NerdyMishka.Extensions.DependencyInjection
{
    public interface IModule
    {
        void Apply(IServiceCollection services);
    }
}