namespace Server
{
    public interface IQueryService
    {
        TResult Query<TResult>(IQuery<TResult> query);
    }
}