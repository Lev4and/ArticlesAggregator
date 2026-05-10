using Messaging.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;

namespace Messaging.Hosting;

public abstract class ConsumerWorker
{
    protected readonly ITracer<ConsumerWorker> Tracer;
    protected readonly ILogger<ConsumerWorker> Logger;
    protected readonly IServiceProvider        ServiceProvider;
    
    public ConsumerWorker(
        ITracer<ConsumerWorker> tracer,
        ILogger<ConsumerWorker> logger,
        IServiceProvider        serviceProvider)
    {
        Tracer          = tracer;
        Logger          = logger;
        ServiceProvider = serviceProvider;
    }
    
    protected virtual async Task RunConsumerAsync(IMessageConsumer consumer, CancellationToken ct = default)
    {
        Logger.LogInformation("Consumer receive started Id: {MessageConsumerId}", consumer.Id);
        
        var loggingContext = new Dictionary<string, object>
        {
            { "MessageConsumerId", consumer.Id },
        };
        
        using var consumerLoggingScope = Logger.BeginScope(loggingContext);

        await foreach (var messageConsumedContext in consumer.ReceiveAsync(ct))
        {
            var message     = messageConsumedContext.Data;
            var messageType = message.GetType();

            using var operation = Tracer.StartOperation("Consume message");
            
            Logger.LogInformation(
                "Consume message Id: {MessageContextId} MessageId: {MessageId} MessageType: {MessageType}", 
                    messageConsumedContext.MessageId, message.Id, messageType);
            
            try
            {
                await using var scope = ServiceProvider.CreateAsyncScope();
                
                var messageHandlerType = typeof(IMessageHandler<>).MakeGenericType(messageType);
                        
                var messageHandler = scope.ServiceProvider.GetService(messageHandlerType);
                if (messageHandler is null)
                {
                    await consumer.AcknowledgeAsync(messageConsumedContext, ct);
                    
                    Logger.LogWarning("Message handler not found MessageType: {MessageType}", messageType);

                    continue;
                }
                        
                var messageHandleResult = await ((IMessageHandler)messageHandler).HandleAsync(message, ct);
                if (!messageHandleResult.IsSuccess)
                {
                    await consumer.NegativeAcknowledgeAsync(messageConsumedContext, ct);
                    
                    var error = messageHandleResult.Errors.First();

                    Logger.LogError(
                        "Message handle completed with errors MessageId: {MessageId} MessageType: {MessageType} ErrorType: {ErrorType} ErrorMessage: {ErrorMessage}",
                            message.Id, messageType, error.Type, error.Message);

                    continue;
                }
                
                await consumer.AcknowledgeAsync(messageConsumedContext, ct);
                
                Logger.LogInformation(
                    "Message handle completed MessageId: {MessageId} MessageType: {MessageType}",
                        message.Id, messageType);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, 
                    "Consume message failed MessageId: {MessageId} MessageType: {MessageType}",
                        message.Id, messageType);
                
                await consumer.NegativeAcknowledgeAsync(messageConsumedContext, ct);
            }
        }
        
        Logger.LogInformation("Consumer finished Id: {MessageConsumerId}", consumer.Id);
    }
}