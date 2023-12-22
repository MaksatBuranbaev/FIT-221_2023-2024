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
        var obj = _endable.obj;
        var properties = _endable.property;
        properties.ToList().ForEach(p => obj.DeleteProperty(p));

        var cmd = _endable.cmd;
        var emptyCommand = IoC.Resolve<ICommand>("Command.Empty");
        IoC.Resolve<IInjectableCommand>("Command.Inject", cmd, emptyCommand);
    }
}
