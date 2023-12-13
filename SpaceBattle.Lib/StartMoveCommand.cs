using Hwdtech;

namespace SpaceBattle.Lib;


public class StartMoveCommand : ICommand {
    private IMoveStartable _order;

    public StartMoveCommand(IMoveStartable order) {
        _order = order;
    }

    public void Execute() {
        IoC.Resolve<ICommand>("OrderTargetSetProperty", "Velocity", _order.initialVelocity).Execute(); 
        IoC.Resolve<ICommand>("ChangeVelocity", _order.UObject, _order.initialVelocity).Execute();
        
        var movable = IoC.Resolve<IMovable>("Commands.Movable.Create", _order);
        var moveCommand = IoC.Resolve<ICommand>("Commands.MoveCommand.Create", movable);

        _order.Queue.Add(moveCommand);
    }
}