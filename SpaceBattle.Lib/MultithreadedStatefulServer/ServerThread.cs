using System.Collections.Concurrent;
using Hwdtech;
namespace SpaceBattle.Lib;

public class ServerThread
{
    private readonly Thread _t;
    private readonly BlockingCollection<ICommand> _q;
    private bool stop = false;
    private Action strategy;
    public ServerThread(BlockingCollection<ICommand> q)
    {
        _q = q;
        strategy = () =>
        {
            var cmd = _q.Take();
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                IoC.Resolve<ICommand>("Exception.Handle", cmd, e).Execute();
            }
        };
        _t = new Thread(() =>
        {
            while (!stop)
            {
                strategy();
            }
        });
    }
    public void Start()
    {
        _t.Start();
    }

    public void Stop()
    {
        stop = true;
    }
    public void UpdateBehaviour(Action act)
    {
        strategy = act;
    }
    public void Add(ICommand cmd)
    {
        _q.Add(cmd);
    }
}
