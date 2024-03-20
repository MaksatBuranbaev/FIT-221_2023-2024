using System.Collections.Concurrent;
using Hwdtech;
namespace SpaceBattle.Lib;

public class ServerThread
{
    private readonly Thread _t;
    private readonly BlockingCollection<ICommand> _q;
    private bool _stop = false;
    private Action _strategy;
    public ServerThread(BlockingCollection<ICommand> q)
    {
        _q = q;
        _strategy = () =>
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
            while (!_stop)
            {
                _strategy();
            }
        });
    }
    public void Start()
    {
        _t.Start();
    }

    public void Stop()
    {
        _stop = true;
    }
    public void UpdateBehaviour(Action act)
    {
        _strategy = act;
    }
    public void Add(ICommand cmd)
    {
        _q.Add(cmd);
    }
    public Thread GetThread()
    {
        return _t;
    }
    public BlockingCollection<ICommand> GetQ()
    {
        return _q;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != typeof(Thread))
        {
            return false;
        }
        else
        {
            return _t == (Thread)obj;
        }
    }
    public override int GetHashCode()
    {
        return _t.GetHashCode();
    }
}
