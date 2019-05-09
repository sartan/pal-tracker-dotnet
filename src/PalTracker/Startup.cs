using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PalTracker
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(ILogger<Startup> logger, IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton(sp =>
            {
                var messageString = Configuration.GetValue<string>("WELCOME_MESSAGE");

                if (messageString == null)
                {
                    _logger.LogInformation("WELCOME_MESSAGE not configured.");
                }

                return new WelcomeMessage(
                    messageString
                );
            });

            services.AddSingleton(sp => new CloudFoundryInfo(
                Configuration.GetValue<string>("PORT"),
                Configuration.GetValue<string>("MEMORY_LIMIT"),
                Configuration.GetValue<string>("CF_INSTANCE_INDEX"),
                Configuration.GetValue<string>("CF_INSTANCE_ADDR")
            ));

            services.AddSingleton<ITimeEntryRepository, InMemoryTimeEntryRepository>();
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
