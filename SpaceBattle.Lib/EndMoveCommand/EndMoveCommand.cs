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
        IoC.Resolve<ICommand>("UObject.DeleteProperty", _endable.obj, _endable.property).Execute();
        var cmd = _endable.cmd;
        var emptyCommand = IoC.Resolve<ICommand>("Command.Empty");
        IoC.Resolve<IInjectableCommand>("Command.Inject", cmd, emptyCommand);
    }
}
