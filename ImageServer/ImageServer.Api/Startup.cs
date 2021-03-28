using ImageServer.Application.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ImageServer.Application.Handlers.Query.GetImage;
using ImageServer.Application.Handlers.Query.GetImage.ImageSavingStrategy;
using MediatR;

namespace ImageServer.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //https://adamstorr.azurewebsites.net/blog/aspnetcore-and-the-strategy-pattern
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(GetImage));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ImageServer.Api", Version = "v1" });
            });

            //https://adamstorr.azurewebsites.net/blog/aspnetcore-and-the-strategy-pattern
            services.AddSingleton(Configuration.GetSection("AppSettings").Get<AppSettings>());
            services.AddScoped<IGetImageService, GetImageService>();
            services.AddScoped<IImageSavingStrategy, ImageSavingStrategy>();
            services.AddScoped<IImageSaver, JpgSaver>();
            services.AddScoped<IImageSaver, PngSaver>();
            services.AddScoped<GetImageRequestValidator, GetImageRequestValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ImageServer.Api v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
