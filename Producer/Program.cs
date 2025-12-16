
using MassTransit;
using Shared.Configuration;

namespace Producer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var config = builder.Configuration.GetSection("RabbitMQConfiguration").Get<RabbitMQConfiguer>();

            // Add Masstransit Service
            builder.Services.AddMassTransit(option =>
            {
                option.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(config.Server, "/", h =>
                    {
                        h.Username(config.UserName);
                        h.Password(config.Password);
                    });
                    cfg.ConfigureEndpoints(context);
                    cfg.Exclusive = false;
                    cfg.Durable = true;
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });


            var app = builder.Build();
       
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();


            app.MapControllers();
            app.UseCors();
            app.Run();
        }
    }
}
