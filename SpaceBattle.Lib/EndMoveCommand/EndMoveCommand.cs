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
        IoC.Resolve<string>("DeleteProperty.UObject", _endable.target, _endable.property);
        var cmd = _endable.cmd;
        var emptyCommand = IoC.Resolve<ICommand>("Command.EmptyCommand");
        IoC.Resolve<IInjectableCommand>("Inject.Command", cmd, emptyCommand);
    }
}
