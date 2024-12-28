using System.Text;
using LoggingService.LogProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LoggingService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogProcessor _logProcessor;
    private IConnection _connection;
    private IModel _channel;
    private string _queueName;

    public MessageBusSubscriber(
        IConfiguration configuration,
        ILogProcessor logProcessor
        )
    {
        _configuration = configuration;
        _logProcessor = logProcessor;
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHostName"] ?? throw new InvalidOperationException(),
            Port = int.Parse(_configuration["RabbitMQPort"] ?? throw new InvalidOperationException()),
            ClientProvidedName = "LoggingService",
        };
        
        _connection = factory.CreateConnection();
        Console.WriteLine("RabbitMQ Connection established");
        
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "amq.topic", type: ExchangeType.Topic, durable: true);
        _queueName = _channel.QueueDeclare().QueueName;
        
        _channel.QueueBind(queue: _queueName, exchange: "user.topic", routingKey: "user.add");
        
        Console.WriteLine("--> Listening for new messages");

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += (ModuleHandle, ea) =>
        {
            Console.WriteLine($"--> Received message: {ea.Body}");
            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
            
            Console.WriteLine($"--> Message encoded: {notificationMessage}");
            
            _logProcessor.ProcessLog(notificationMessage);
        };
        
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        
        return Task.CompletedTask;
    }

    private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> Shutting down the RabbitMQ connection");
    }
    
    public override void Dispose()
    {
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }
}