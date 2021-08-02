using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MoviesApi.Data;
using MoviesApi.Entities;

namespace MoviesApi
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
            services.AddDbContext<DataContext>(x =>{
                x.UseSqlServer(Configuration.GetConnectionString("MyConnection"));
            });
            services.AddIdentity<ApplicationUser,ApplicationRole>(
                option=>{
                    option.Password.RequireDigit=true;
                    option.Password.RequireLowercase=true;
                    option.Password.RequireNonAlphanumeric=true;
                    option.Password.RequiredUniqueChars=1;
                    option.Password.RequireUppercase=true;
                    option.Password.RequiredLength=6;
                    option.SignIn.RequireConfirmedEmail=true;
                    option.Lockout.MaxFailedAccessAttempts=5; //default 5
                    option.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(15);

                }
            ).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MoviesApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MoviesApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
