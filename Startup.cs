using CarAppDotNetApi.Data;
using CarAppDotNetApi.ErrorHandling;
using CarAppDotNetApi.Extensions;
using CarAppDotNetApi.Repositories;
using CarAppDotNetApi.Repositories.implementation;
using CarAppDotNetApi.Security;
using CarAppDotNetApi.Services;
using CarAppDotNetApi.Services.ServiceImplementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CarAppDotNetApi
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
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(Configuration.GetConnectionString("CarAppConnection"))
            );

            services.AddTransient<ICarRepository, CarRepositoryImpl>();
            services.AddTransient<IUserRepository, UserRepositoryImpl>();
            services.AddTransient<IModelRepository, ModelRepositoryImpl>();
            
            services.AddTransient<IAuthService, AuthServiceImpl>();
            services.AddTransient<ICarService, CarServiceImpl>();
            services.AddTransient<TokenCreator>();
            services.AddTransient<AuthorizeFilter>();
            services.AddAutoMapper(typeof(Startup));
            services.AddSwaggerExtension(Configuration);
            services.AddCarAppAuthentication(Configuration);
            services.AddMvc();
            services.AddControllers();
            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CarAppDotNetApi v1"));
            }
            
            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
