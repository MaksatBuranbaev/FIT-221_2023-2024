namespace SpaceBattle.Lib;
using Hwdtech;
using WebHttp;
public class Endpoint : ICommand
{
    internal GameContract? _gameobj { get; set; }
    internal string _game_id {get; set;}
    internal Endpoint(GameContract gameobj)
    {
        _gameobj = gameobj;
        _game_id = gameobj.game_id;
    }
    public void Execute()
    {
        var mp = IoC.Resolve<ICommand>("Command.Interpreted", _gameobj);
        var t = IoC.Resolve<ICommand>("GetThread", _game_id);
        IoC.Resolve<ICommand>("SendCommand", t, mp);
    }
}
