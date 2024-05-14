namespace SpaceBattle.Lib;
public class DefaultExceptionHandler : IExceptionHandler
{
    private readonly Exception _e;
    public DefaultExceptionHandler(Exception e)
    {
        _e = e;
    }
    public void Handle()
    {
        throw _e;
    }
}
