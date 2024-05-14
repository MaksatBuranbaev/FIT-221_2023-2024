using System.Diagnostics;
using Hwdtech;
namespace SpaceBattle.Lib;

public class GameCommand : ICommand
{
    private readonly object _scope;
    private readonly Queue<ICommand> _queue;
    public GameCommand(object scope, Queue<ICommand> queue)
    {
        _scope = scope;
        _queue = queue;
    }
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", _scope).Execute();

        var stopWatch = new Stopwatch();
        stopWatch.Start();

        while (stopWatch.ElapsedMilliseconds < IoC.Resolve<int>("Quantum.Get"))
        {
            var cmd = _queue.Dequeue();
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                IoC.Resolve<IExceptionHandler>("ExceptionHandler.Find", cmd, e);
            }
        }

        stopWatch.Stop();
    }
}
