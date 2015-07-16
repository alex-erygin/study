namespace Server
{
    public interface ICommandService
    {
        void Execute(ICommand command);
    }
}