using System;
using System.Threading.Tasks;
using MassTransit;

namespace MassTransitExample
{
    public interface IMessage
    {
        string Text { get; }
    }

    public class MessageConsumer : IConsumer<IMessage>
    {
        public async Task Consume(ConsumeContext<IMessage> context)
        {
            await Console.Out.WriteLineAsync($"Получено сообщение: {context.Message.Text}");
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("message_queue", ep =>
                {
                    ep.Consumer<MessageConsumer>();
                });
            });

            await busControl.StartAsync(); 

            Console.WriteLine("Ожидание сообщений. Нажмите любую клавишу для завершения.");
            Console.ReadKey();

            await busControl.StopAsync(); 
        }
    }
}
