using System;
using System.Text;
using RabbitMQ.Client;

namespace Auth_Service.Web.Logic
{
    public class EventbusSend
    {
        public void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "auth_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                var message = GetMessageCreateUser(args);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.CorrelationId = Guid.NewGuid().ToString();

                channel.BasicPublish(exchange: "", routingKey: "auth_queue", basicProperties: properties, body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessageCreateUser(string[] args)
        {
            string message = "Create new User";
            return ((args.Length > 0) ? string.Join(" ", args) : message);
        }
    }
}
