using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JinZhou.Middlewares;
using JinZhou.Models;
using JinZhou.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace JinZhou
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
            services.AddMvc();
            services.Configure<WxConfig>(this.Configuration.GetSection("WxConfig"));
            services.AddDbContextPool<JzDbContext>(options => options.UseMySql(
                    "Server=localhost;Database=jinzhoudb;User=jinzhoudb;Password=arLRxsxPfGwRB2YB;",
                    mySqlOptions => mySqlOptions.ServerVersion(new Version(5, 7, 0), ServerType.MySql)
                )
            );
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
                app.UseExceptionHandler("/Home/Error");
            }

            //app.UseMiddleware<RequestMiddlewares>();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
