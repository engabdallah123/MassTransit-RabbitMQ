using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shared.Configuration;
using System; 

namespace Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // create builder 
            var builder = Host.CreateDefaultBuilder(args)
                                .ConfigureAppConfiguration((content, config) =>
                                {
                                    config.SetBasePath(AppContext.BaseDirectory);
                                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                                });

            builder.ConfigureServices((context, services) =>
            {
                var rabbitMQConfig = context.Configuration.GetSection("RabbitMQConfiguration").Get<RabbitMQConfiguer>();
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<ConsumerDemo>()
                     .Endpoint(e => e.Name = rabbitMQConfig.QueueName);

                    x.UsingRabbitMq((ctx, cfg) =>
                    {
                        cfg.Host(rabbitMQConfig.Server, "/", h =>
                        {
                            h.Username(rabbitMQConfig.UserName);
                            h.Password(rabbitMQConfig.Password);
                        });
                        cfg.ConfigureEndpoints(ctx);
                        cfg.Exclusive = false;
                        cfg.Durable = true;
                    });
                });

            });

            builder.Build().Run();
        }
    }
}
