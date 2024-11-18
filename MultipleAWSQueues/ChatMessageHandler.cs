using AWS.Messaging;

namespace MultipleAWSQueues;

public class ChatMessage
{
    public string MessageDescription { get; set; } = string.Empty;
}
public class ChatMessageHandler : IMessageHandler<ChatMessage>
{
    public Task<MessageProcessStatus> HandleAsync(MessageEnvelope<ChatMessage> messageEnvelope, CancellationToken token = default)
    {
        if (messageEnvelope == null)
        {
            return Task.FromResult(MessageProcessStatus.Failed());
        }

        if (messageEnvelope.Message == null)
        {
            return Task.FromResult(MessageProcessStatus.Failed());
        }

        var message = messageEnvelope.Message;

        Console.WriteLine($"Message Description: {message.MessageDescription}");

        return Task.FromResult(MessageProcessStatus.Success());
    }
}