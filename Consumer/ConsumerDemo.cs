using MassTransit;
using Shared.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    public class ConsumerDemo : IConsumer<MessageDemo>
    {
        public async Task Consume(ConsumeContext<MessageDemo> context)
        {
            var message = context.Message;
            Console.WriteLine($"Received Message:\n From: {message.From}\n To: {message.To}\n Subject: {message.Title}\n Content={message.Content}");
            await Task.CompletedTask;
        }
    }

}
