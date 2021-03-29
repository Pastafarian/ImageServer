using System.Text.Json.Serialization;
using ImageServer.Application.Config;
using ImageServer.Application.Handlers.Query.GetImage;
using ImageServer.Application.Handlers.Query.GetImage.ImageSavingStrategy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ImageServer.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            services.AddMediatR(typeof(GetImage));
            services.AddMemoryCache(options =>
            {
                options.CompactionPercentage = 0.1d;
                options.SizeLimit = appSettings.MaximumCacheSizeInKb;
            });

            services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ImageServer.Api", Version = "v1" });
            });

        
            services.AddSingleton(Configuration.GetSection("AppSettings").Get<AppSettings>());
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IImageCachingService, ImageCachingService>();
            services.AddScoped<IImageSavingStrategy, ImageSavingStrategy>();
            services.AddScoped<IImageSaver, JpgSaver>();
            services.AddScoped<IImageSaver, PngSaver>();
            services.AddScoped<IImageProcessor, ImageProcessor>();
            services.AddScoped<GetImageRequestValidator, GetImageRequestValidator>();
        }

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
