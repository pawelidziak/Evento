using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evento.Api.Framework;
using Evento.Core.Repositories;
using Evento.Infrastructure.Mappers;
using Evento.Infrastructure.Repositories;
using Evento.Infrastructure.Services;
using Evento.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


namespace Evento.Api
{
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
            services.AddMvc()
                .AddJsonOptions(x => x.SerializerSettings.Formatting = Formatting.Indented);

            services.AddMemoryCache();

            // dodane
            services.AddAuthorization();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDataInitializer, DataInitializer>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddSingleton<IJwtHandler, JwtHandler>();

            services.AddSingleton(AutoMapperConfig.Initialize());


            services.Configure<JwtSettings>(Configuration.GetSection("jwt"));
            services.Configure<AppSettings>(Configuration.GetSection("app"));

            var config = Configuration.GetSection("jwt").Get<JwtSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            .AddJwtBearer(options =>
            {
                // options.Authority = "http://localhost:5000/";
                // options.Audience = "resource-server";
                options.RequireHttpsMetadata = false;
                // options.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("jwtSettings.Value.Ke"));
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "http://localhost:5000", // better: config.Issuer
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_123!")) // better: config.Key

                };
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication(); // JWT

            // var jwtSettings = app.ApplicationServices.GetService<JwtSettings>();
            // app.UseJwtBearerAuthentication(new JwtBearerOptions
            // {
            //     AutomaticAuthenticate = true,
            //     TokenValidationParameters = new TokenValidationParameters
            //     {
            //         ValidIssuer = jwtSettings.Issuer;
            //         ValidateAudience = false,
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
            //     }
            // });
           
            SeedData(app);
            app.UseErrorHandler();
            app.UseMvc();

        }

        private void SeedData(IApplicationBuilder app)
        {
            var settings = app.ApplicationServices.GetService<IOptions<AppSettings>>();
            if (settings.Value.SeedData)
            {
                var dataInitializer = app.ApplicationServices.GetService<IDataInitializer>();
                dataInitializer.SeedAsync();
            }
        }
    }
}
