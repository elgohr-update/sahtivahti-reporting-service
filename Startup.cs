using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportingService.EventStore;
using ReportingService.Model;
using ReportingService.Mq;

namespace ReportingService
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o => o.EnableEndpointRouting = false);

            services.Configure<MqConfig>(_configuration.GetSection("Mq"));
            services.AddTransient<IMq, RabbitMq>();

            services.Configure<EventStoreConfig>(_configuration.GetSection("EventStore"));
            services.AddTransient<IEventStore, EventStoreClient>();
            services.AddTransient<IEventStoreProjections, EventStoreProjections>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMq mq, IEventStore eventStore)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            mq.Consume<RecipeActionEvent>("", recipe =>
            {
                var a = recipe.Type switch
                {
                    "recipe.created" => eventStore.Publish("recipeCreated", new {}),
                    "recipe.updated" => eventStore.Publish("recipeUpdated", new {}),
                    _ => throw new InvalidOperationException("Can't find suitable action")
                };

                return true;
            });
        }
    }
}
