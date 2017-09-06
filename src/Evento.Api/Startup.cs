using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                .AddJsonOptions(x=> x.SerializerSettings.Formatting = Formatting.Indented);

            // dodane
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IUserService, UserService>();

            services.AddSingleton(AutoMapperConfig.Initialize());

            // services.Configure<JwtSettings>(Configuration.GetSection("jwt"));

            // services.AddAuthentication(options =>
            // {
            //     options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            // })

            // .AddJwtBearer(options =>
            // {
            //     options.Authority = "http://localhost:30940/";
            //     options.Audience = "resource-server";
            //     options.RequireHttpsMetadata = false;
            // });
            var jwtSettings = Configuration.GetSection("jwt").Get<JwtSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters()
                {

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[jwtSettings.Key])),

                    ValidateAudience = false,
                    // ValidAudience = Configuration["JWT:ValidIssuer"],

                    ValidateIssuer = true,
                    ValidIssuer = Configuration[jwtSettings.Issuer],

                    // ValidateLifetime = true,
                    // ClockSkew = TimeSpan.Zero
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


            app.UseMvc();
        }
    }
}
