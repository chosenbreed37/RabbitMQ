using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SubscriberApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    // Declare the exchange
                    channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);

                    // Declare anonymous/temporary queue
                    var queueName = channel.QueueDeclare().QueueName;
                    var routingKey = args[0];

                    // Bind to the exchange
                    channel.QueueBind(queue: queueName, exchange: "topic_logs", routingKey: routingKey);

                    Console.WriteLine("[*] Waiting for logs...");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, dlvrArgs) =>
                    {
                        var body = dlvrArgs.Body;
                        var routingKey1 = dlvrArgs.RoutingKey;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0} : {1}", routingKey1, message);
                    };

                    channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
                    Console.ReadLine();
                }
            }
        }
    }
}
