using System.Text;
using Dotnet.JsonIdentityProvider.IdentityProvider;
using Dotnet.JsonIdentityProvider.IdentityProvider.Model;
using Dotnet.JsonIdentityProvider.IdentityProvider.StorageProvider;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace dotnet.JsonIdentityProvider.Services
{
    public static class ServicesConfiguration
    {
        public static void AddJsonIdentityProvider(this IServiceCollection services, IConfiguration config)
        {
            // This is use to setup the Identity provider and specifying what model should be used by the Identity engine
            services.AddIdentity<ApiUser, IdentityRole>().AddDefaultTokenProviders();

            // Provide a definition of the IUserStore implementation using custom model and store implementation
            // The service is passed through DI to the Identity Builder
            services.AddTransient<IUserStore<ApiUser>, ApiUserStore>();

            // This section is used to provide description of how JWT should be handle
            // As we use MVC as an api and not a website, we must specify how the server is going to respond
            services.AddAuthentication(options =>
            {
                // Specify that the default auth scheme should be 'bearer' as we are using JWT for authentification
                // For example API should return a 401 code there no or incirrect token in the header
                // However a website would redirect to a login page, which, for an API, does not exists and would result in a 404 code instead of 401
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // We also must provide information on how the server should verify a bearer token
            .AddJwtBearer(parameters =>
                {
                    parameters.RequireHttpsMetadata = false;
                    parameters.SaveToken = true;
                    parameters.TokenValidationParameters = new TokenValidationParameters // those are use to define how the token is validated
                    {
                        ValidIssuer = config["Token:issuer"],
                        ValidAudience = config["Token:audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:secretKey"])),
                        ValidateLifetime = true
                    };
                });

            services.AddSingleton<JsonUserStore>();
        }
    }
}