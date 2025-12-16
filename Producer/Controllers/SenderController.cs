using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Message;

namespace Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SenderController : ControllerBase
    {
        private readonly IBus bus;
        private readonly IConfiguration configuration;

        public SenderController(IBus bus,IConfiguration configuration)
        {
            this.bus = bus;
            this.configuration = configuration;
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDemo message)
        {
            try
            {
                var rabbitMQConfig = configuration.GetSection("RabbitMQConfiguration");
                var queueName = rabbitMQConfig.GetValue<string>("QueueName");

                var sendEndpoint = await bus.GetSendEndpoint(new Uri($"queue:{queueName}"));
                await sendEndpoint.Send(message);

                return Ok("Message sent successfully");
            }
            catch
            {
                return StatusCode(500, "An error occurred while sending the message");
            }
        }
    }
}
