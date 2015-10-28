using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Publisher
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
                    channel.ExchangeDeclare("logs", ExchangeType.Fanout);

                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }        
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0)
                   ? string.Join(" ", args)
                   : "info: Hello World!");
        }
    }
}
