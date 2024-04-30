using System.Threading;
using Hwdtech;

namespace SpaceBattle.Server;
public class Endpoint : Lib.ICommand
{
    internal GameContract? _gameobj { get; set; }
    internal string _game_id { get; set; }
    public Endpoint(GameContract gameobj)
    {
        _gameobj = gameobj;
        _game_id = gameobj.game_id;
    }
    public void Execute()
    {
        var mp = IoC.Resolve<Lib.ICommand>("Command.Interpreted", _gameobj);
        var t = IoC.Resolve<Thread>("GetThread", _game_id);
        IoC.Resolve<Lib.ICommand>("SendCommand", t, mp).Execute();
    }
}
