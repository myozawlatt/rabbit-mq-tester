using RabbitMQTester;

Welcome();

while (true)
{
    try
    {
        Console.WriteLine(@"Connection string:
(Sample: host=localhost;virtualHost=Dev;username=guest;password=guest)");
        Console.Write(">");
        var connectionStr = Console.ReadLine().Replace(" ", string.Empty);
        var connectionString = GetConectionString(connectionStr);

        Console.WriteLine("Message (whatever):");
        Console.Write(">");
        var message = Console.ReadLine();

        bool connectionOk = ConnectionTester.Test(connectionString);

        if (connectionOk)
            await Publisher.SendAsync(connectionString, message);

        await Task.Delay(1000);
        Console.WriteLine(Environment.NewLine);
        Console.WriteLine("Finished!");
        Console.WriteLine(Environment.NewLine);
        Console.Write("Do you want to test again (y/n)? :");
        var decision = Console.ReadLine()?.ToLower();
        if (decision != "y")
            break;

        Console.Clear();
        Welcome();
    }
    catch (Exception ex)
    {
        Message.WriteError(ex.Message);
    }
}

void Welcome()
{
    Console.Title = "RabbitMQ Tester (🐇)";
    Console.WriteLine("Welcome to RabbitMQ tester!");
    Console.WriteLine(Environment.NewLine);
}
ConnectionString GetConectionString(string connectionStr)
{
    try
    {
        (string host, string vHost, string username, string password) = GetConnectionStringValues(connectionStr);
        return new(host, vHost, username, password);
    }
    catch
    {
        throw new Exception(@"Invalid connection string.
Connection string should like :
""host=localhost;virtualHost=Dev;username=guest;password=guest""");
    }
}
static (string host, string vHost, string username, string password) GetConnectionStringValues(string rabbitMQConnectionString)
{
    if (string.IsNullOrWhiteSpace(rabbitMQConnectionString))
        throw new ArgumentNullException(nameof(rabbitMQConnectionString));
    var kvp = rabbitMQConnectionString.Split(';')
        .Select(x =>
        {
            var pair = x.Split('=');
            return new KeyValuePair<string, string>(pair.FirstOrDefault(), pair.LastOrDefault());
        })
        .ToList();

    string factory(string key) => kvp.Where(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Single().Value;

    return (factory("host"), factory("virtualHost"), factory("username"), factory("password"));
}