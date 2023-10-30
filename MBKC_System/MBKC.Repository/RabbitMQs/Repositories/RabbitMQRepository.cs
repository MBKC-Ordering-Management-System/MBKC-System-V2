using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

namespace MBKC.Repository.RabbitMQs.Repositories
{
    public class RabbitMQRepository
    {
        public RabbitMQRepository()
        {

        }

        private Models.RabbitMQ GetRabbitMQ()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            return new Models.RabbitMQ()
            {
                HostName = configuration.GetSection("RabbitMQ:HostName").Value,
                Username = configuration.GetSection("RabbitMQ:Username").Value,
                Password = configuration.GetSection("RabbitMQ:Password").Value,
                VirtualHost = configuration.GetSection("RabbitMQ:VirtualHost").Value,
                URI = configuration.GetSection("RabbitMQ:URI").Value,
            };
        }

        public void SendMessage(string message, string queueName)
        {
            try
            {
                Models.RabbitMQ rabbitMQ = GetRabbitMQ();
                ConnectionFactory connectionFactory = new ConnectionFactory()
                {
                    HostName = rabbitMQ.HostName,
                    UserName = rabbitMQ.Username,
                    Password = rabbitMQ.Password,
                    VirtualHost = rabbitMQ.VirtualHost,
                    Uri = new Uri(rabbitMQ.URI)
                };

                IConnection connection = connectionFactory.CreateConnection();

                using IModel channel = connection.CreateModel();
                channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                byte[] body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", queueName, body: body);

                channel.Close();
                connection.Close();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
