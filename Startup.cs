using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.FileProviders;


using Microsoft.AspNetCore.Http;


namespace LTCH_API
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
            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson();
            services.AddMvc();
            //  services.AddCors(x => x.AddPolicy("MVRCors", y => y.AllowCredentials().AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //    builder =>
            //    {
            //        builder.AllowAnyOrigin()
            //               .AllowAnyHeader()
            //               .AllowAnyMethod();
            //    });
            //});
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                policy =>
                {
                    policy.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();
     

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(env.ContentRootPath, @"XingUpdateFile")
            ),
                RequestPath = new PathString("/XingUpdateFile"),
                EnableDirectoryBrowsing = true
            });

        }

        //public static IConfigurationRoot ReadFromAppSettings()
        //{
        //    return new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json", false)
        //        .Build();
        //}

        public static string ReadFromAppSettings(string AppSettingName)
        {
            IConfiguration _config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", false)
                            .Build();

            return _config[AppSettingName];
        }

    }
}
