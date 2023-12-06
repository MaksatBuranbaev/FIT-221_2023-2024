using Hwdtech;

namespace SpaceBattle.Lib;

public class EndMoveCommand : ICommand
{
    private readonly IEndable _endable;

    public EndMoveCommand(IEndable endable)
    {
        _endable = endable;
    }

    public void Execute()
    {
        var cmd = _endable.cmd;
        var emptyCommand = IoC.Resolve<ICommand>("Command.EmptyCommand");
        IoC.Resolve<IInjectableCommand>("Command.Inject", cmd, emptyCommand);
    }
}
