namespace Techfin.Transaction.Infra;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message);
}
