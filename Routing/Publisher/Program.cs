using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

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
                    channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

                    var severity = args[0];
                    var message = args[1];
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "direct_logs", routingKey: severity, basicProperties: null, body: body);
                    Console.WriteLine(" [x] Sent {0}: {1}", severity, message);
                }
            }
        }
    }
}
