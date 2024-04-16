namespace SpaceBattle.Lib;

using Hwdtech;
using WebHttp;
public class Endpoint: ICommand
{
    internal GameContract? gameobj {get; set;}
    public void Execute()
    {
        var mp = IoC.Resolve<ICommand>("MP", gameobj);
        IoC.Resolve<ICommand>("SendCommand", Thread.CurrentThread, mp);
    }
}