using Hwdtech;

namespace SpaceBattle.Lib;

public class StartMoveCommand : ICommand
{
    private readonly IMoveStartable _order;

    public StartMoveCommand(IMoveStartable order)
    {
        _order = order;
    }

    public void Execute()
    {
        IoC.Resolve<ICommand>("OrderTargetSetProperty", _order.UObject, "Velocity", _order.initialVelocity).Execute();

        var movable = IoC.Resolve<IMovable>("Commands.Movable.Create", _order);
        var moveCommand = IoC.Resolve<ICommand>("Commands.MoveCommand.Create", movable);

        var injectCommand = IoC.Resolve<InjectCommand>("Inject.Create", moveCommand);
        IoC.Resolve<ICommand>("Queue.Add", _order.Queue, injectCommand).Execute();
    }
}
