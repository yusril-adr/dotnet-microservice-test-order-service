
namespace DotNetOrderService.Infrastructure.Subscriptions
{
    public interface ISubscriptionActionAsync<in T>
    {
        Task HandleAsync(T data);
    }

    public interface IReplyAsyncAction<T, R>
    {
        Task<R> ReplyAsync(T data);
    }

    public interface ISubscriptionAction<in T>
    {
        void Handle(T data);
    }

    public interface IReplyAction<in T, out R>
    {
        R Reply(T data);
    }
}