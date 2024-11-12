using RabbitMQ.Client;

namespace RabbitMQTester
{
    internal class ConnectionTester
    {
        public  static bool Test(ConnectionString connection)
        {
            try
            {
                (string host, string vHost, string username, string password) = connection;

                var factory = new ConnectionFactory()
                {
                    HostName = host,
                    VirtualHost = vHost,
                    UserName = username,
                    Password = password,
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
                };

                using var conn = factory.CreateConnection();
                return true;
            }
            catch (Exception ex)
            {
                Message.WriteError(ex.Message);
                return false;
            }
        }
    }
}
