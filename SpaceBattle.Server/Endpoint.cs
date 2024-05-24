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
        IoC.Resolve<Lib.ICommand>("SendCommand", _gameobj.game_id, mp).Execute();
    }
}
