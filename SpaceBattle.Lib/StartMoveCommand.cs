using Hwdtech;

namespace SpaceBattle.Lib;

/* 
// IoC.Register
ICommand registerMC = IoC.Resolve<ICommand>(
    "IoC.Register", 
    "Commands.MoveCommand.Create", 
    (object[] args)=> {
        return new MoveCommand(args[0]);
    });

registerMC.Execute();

1. Создание нового
2. Полдучение из пула
3. Синглтон
4. ThreadLocal */

class StartMoveCommand : ICommand {
    private IMoveStartable _order;

    public StartMoveCommand(IMoveStartable order) {
        _order = order;
    }

    public void Execute() {
       /*  1. Устанвить скорость в таргет
            // order.target.set_property("Velocity", order.initialVelocity)
            IoC.Resolve<ICommand>("ChangeVelocity", order.UObject, order.initialVelocity).Execute();
        2. Создать операцию движения
        3. записать операцию в таргет
        4. Закинуть команду в очередь */

        IoC.Resolve<ICommand>("OrderTargetSetProperty", _order.UObject, "Velocity", _order.initialVelocity).Execute();
        IoC.Resolve<ICommand>("ChangeVelocity", _order.UObject, _order.initialVelocity).Execute();

        var movable = IoC.Resolve<IMovable>("Commands.Movable.Create", _order);
        var moveCommand = IoC.Resolve<ICommand>("Commands.MoveCommand.Create", movable);

        _order.Queue.Add(moveCommand);
    }
}