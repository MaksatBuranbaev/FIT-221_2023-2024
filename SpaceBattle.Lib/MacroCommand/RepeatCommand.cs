namespace SpaceBattle.Lib;
using Hwdtech;

public class RepeatCommand : ICommand
{
    private readonly List<ICommand> _cmd = new() { };

    public void Add(ICommand cmd)
    {
        _cmd.Add(cmd);
    }
    public void Execute()
    {
        IoC.Resolve<ICommand>("Queue.Add", _cmd).Execute();
    }
}
