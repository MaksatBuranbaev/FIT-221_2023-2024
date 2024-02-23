namespace SpaceBattle.Lib;
using Hwdtech;

public class RepeatCommand : ICommand
{
    private ICommand _cmd;

    public RepeatCommand(ICommand cmd)
    {
        _cmd = cmd;
    }
    public void Execute()
    {
        IoC.Resolve<ICommand>("Queue.Add", _cmd).Execute();
    }
}
