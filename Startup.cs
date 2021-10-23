using AutoMapper;
using HotelListing.Configurations;
using HotelListing.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListing.IRepository;
using HotelListing.Repository;
using HotelListing.Services;

namespace HotelListing
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
            // services.AddRazorPages();

            services.AddDbContext<DatabaseContext>(options => 
                 options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
            );

            // Authentication
            services.AddAuthentication();
            // Abstract config to ServiceExtensions.cs file
            services.ConfigureIdentity();
            // JWT
            services.ConfigureJWT(Configuration);

            // CORS Configuration
            // Who is allowed to access this API, what methods are available and what headers must the user have
            services.AddCors(o => {
                o.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            /* Auto Mapper */
            services.AddAutoMapper(typeof(MapperInitializer));

            /* Register IUnitOfWork */
            // AddTransient - provide a fresh copy every time a client contacts the server
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // Auth - Register Service AuthManager
            services.AddScoped<IAuthManager, AuthManager>();

            /* Swagger */
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
            });

            // NewSoft => Ignore some Loop Reference Warnings
            services.AddControllers().AddNewtonsoftJson(op 
                => op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            /* Swagger */
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));

            // Error Handling
            app.ConfigureExceptionHandler();

            // CORS POLICY, user specific Policy we defined in ConfigureServices
            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            //app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
