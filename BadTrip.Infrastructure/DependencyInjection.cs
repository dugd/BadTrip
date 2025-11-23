using BadTrip.Application.Common.Interfaces;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using BadTrip.Infrastructure.Authentication;
using BadTrip.Infrastructure.Persistence;
using BadTrip.Infrastructure.Persistence.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace BadTrip.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Get connection string from config
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddRepositories();

            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),

                        ClockSkew = TimeSpan.Zero,
                    };
                });

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            var repositories = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Repository"));

            foreach (var repoType in repositories)
            {
                var interfaceType = repoType.GetInterfaces()
                    .FirstOrDefault(i => i.Name == $"I{repoType.Name}");

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, repoType);
                }
            }

            return services;
        }
    }
}
