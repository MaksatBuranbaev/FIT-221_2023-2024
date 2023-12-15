namespace SpaceBattle.Lib;

public class InjectCommand : ICommand, IInjectableCommand
{
    private ICommand _cmd;

    public InjectCommand(ICommand cmd)
    {
        _cmd = cmd;
    }

    public void Execute()
    {
        _cmd.Execute();
    }

    public void Inject(ICommand cmd)
    {
        _cmd = cmd;
    }
}
