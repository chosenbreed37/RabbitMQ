using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace UnroutableMessages
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

                    channel.BasicReturn += (model, e) => {
                        var routingKey = e.RoutingKey;
                        var data = Encoding.UTF8.GetString(e.Body);
                        Console.WriteLine(" [x] Unroutable message {0} : {1}", routingKey, data);
                    };

                    var severity = args[0];
                    var message = args[1];
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "topic_logs", routingKey: severity, mandatory: true, basicProperties: null, body: body);
                    Console.WriteLine(" [x] Sent {0}: {1}", severity, message);
                }
            }
        }
    }
}
