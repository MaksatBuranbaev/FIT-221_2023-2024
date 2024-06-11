namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateStartMovementCommandStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var obj = (IUObject)args[0];
        return new StartMoveCommand(IoC.Resolve<IMoveStartable>("Adapter.Create", obj, typeof(IMoveStartable)));
    }
}
