namespace SpaceBattle.Lib;
using Hwdtech;
using WebHttp;
public class Endpoint : ICommand
{
    internal GameContract? _gameobj { get; set; }
    internal Endpoint(GameContract gameobj)
    {
        _gameobj = gameobj;
    }
    public void Execute()
    {
        var mp = IoC.Resolve<ICommand>("MP", _gameobj);
        var t = IoC.Resolve<ICommand>("CurrentThread");
        IoC.Resolve<ICommand>("SendCommand", t, mp);
    }
}
