namespace Server
{
    public interface IQueryHandler<TQuery, TResult> : IQueryHandler where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }

    public interface IQueryHandler
    {
    }
}