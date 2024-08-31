using ESB.Messages.Responses;
using MassTransit;

namespace ESB.Infrastructure.Consumers;

public class TestConsumer : IConsumer<TestResponse>
{
    public async Task Consume(ConsumeContext<TestResponse> context)
    {
        Console.WriteLine("Consumed it");
        await Task.CompletedTask;
    }
}