using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BadTrip.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {   
            // Register MediaR handlers
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            });

            // Register Fluent validators
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            // Pipeline behaviors?

            return services;
        }
    }
}
