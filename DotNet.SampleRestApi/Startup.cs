using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DotNet.SampleRestApi.Repositories;
using DotNet.SampleRestApi.Dtos;
using DotNet.SampleRestApi.Entities;
using Swashbuckle.AspNetCore.Swagger;

namespace DotNet.SampleRestApi
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
        services.AddOptions();
            //DataContextimizi burada ayarlıyoruz ve appsetting.json dosyasındaki connectionstring ile ilişkilendirme yapıyoruz.
            //services.AddDbContext<LogDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("LogDatabase")));
            services.AddDbContext<LogDbContext>(opt => opt.UseInMemoryDatabase("LogDatabase"));
            services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddScoped<IUrlHelper>(implementationFactory =>
        {
            var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
            return new UrlHelper(actionContext);
        });

        services.AddMvcCore().AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        services.AddApiVersioning(config =>
        {
            config.AssumeDefaultVersionWhenUnspecified = true;
        });
        services.AddSwaggerGen(
            options =>
            {
                var provider = services.BuildServiceProvider()
                                    .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                        description.GroupName,
                        new Info()
                        {
                            Title = $"Sample API {description.ApiVersion}",
                            Version = description.ApiVersion.ToString()
                        });
                }
            });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory,
        IHostingEnvironment env, IApiVersionDescriptionProvider provider)
    {
        loggerFactory.AddConsole();
        app.UseHsts();
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (errorFeature != null)
                {
                    var logger = loggerFactory.CreateLogger("Global exception logger");
                    logger.LogError(500, errorFeature.Error, errorFeature.Error.Message);
                }

                await context.Response.WriteAsync("There was an error");
            });
        });

        //app.AddSeedData();

        app.UseHttpsRedirection();

        app.UseSwagger();
        app.UseSwaggerUI(
            options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });

        app.UseCors("AllowAllOrigins");
        AutoMapper.Mapper.Initialize(mapper =>
        {
            mapper.CreateMap<Log, LogDto>().ReverseMap();
        });

        app.UseMvc();
    }
}
}
