namespace Dotnet.JsonIdentityProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpsPolicy;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Dotnet.JsonIdentityProvider.IdentityProvider;
    using Dotnet.JsonIdentityProvider.IdentityProvider.Model;
    using Dotnet.JsonIdentityProvider.IdentityProvider.StorageProvider;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
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
            .AddJwtBearer(config =>
                {
                    config.RequireHttpsMetadata = false;
                    config.SaveToken = true;
                    config.TokenValidationParameters = new TokenValidationParameters // those are use to define how the token is validated
                    {
                        ValidIssuer = Configuration["Token:issuer"],
                        ValidAudience = Configuration["Token:audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:secretKey"])),
                        ValidateLifetime = true
                    };
                });

            services.AddAuthorization(config =>
            {
                config.AddPolicy("SuperUsers", policy => policy.RequireClaim("SuperUser", "True"));
            });

            services.AddSingleton<JsonUserStore>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
