using System.Threading;
using Hwdtech;

namespace SpaceBattle.Server;
public class Endpoint : Lib.ICommand
{
    internal GameContract _gameobj { get; set; }
    public void Set(GameContract gameobj)
    {
        _gameobj = gameobj;
    }
    public void Execute()
    {
        var mp = IoC.Resolve<Lib.ICommand>("Command.Interpreted", _gameobj);
        var t = IoC.Resolve<Thread>("GetThread", _gameobj.game_id);
        IoC.Resolve<Lib.ICommand>("SendCommand", t, mp).Execute();
    }
}
