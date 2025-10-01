using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OrderConsumer.Controllers;
using System;

namespace OrderConsumer
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
            //Add configuration of mensaggeria
            services.AddMassTransit(x =>
            {
                x.AddConsumer<Consumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.Host(new Uri("rabbitmq://localhost"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    /*Queue of name que irá ser consumida
                     Configura uma receive endpoint usa a fila QueueOfName e define o comportamento de consumo dessa fila.
                     */
                    config.ReceiveEndpoint("QueueOfName", ep =>
                    {
                        //Define a qtde de mensagens que o RabbitMQ pode entregar antes de aguardar a confirmação.
                        ep.PrefetchCount = 10;

                        //Se houver uma execeção, o MassTransit tenta novamente, 2 tentativas com intervalo de 100ms
                        ep.UseMessageRetry(r => r.Interval(2, 100));
                        ep.ConfigureConsumer<Consumer>(provider);
                    });
                }));
            });

            //services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderConsumer", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderConsumer v1"));
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
