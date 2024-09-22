using Microsoft.Extensions.DependencyInjection;
using ToDoList.Services.Implementations;
using ToDoList.Services.Interfaces;

namespace ToDoList.Services
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            return services;
        }
    }
}
