using System;
using System.Threading.Tasks;
using MassTransit;

namespace MassTransitExample;

public interface IMessage
{
    string Text { get; }
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
        });

        await busControl.StartAsync(); // Запускаем шину сообщений

        try
        {
            while (true)
            {
                string str = Console.ReadLine();
                if (str == "q")
                {
                    break;
                }

                var endpoint = await busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/message_queue"));
                await endpoint.Send<IMessage>(new { Text = str });
            }

        }
        finally
        {
            await busControl.StopAsync(); // Останавливаем шину сообщений
        }
    }
}
