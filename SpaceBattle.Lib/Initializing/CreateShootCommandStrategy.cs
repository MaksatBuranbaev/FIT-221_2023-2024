namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateShootCommandStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var obj = (IUObject)args[0];
        return new ShootCommand(IoC.Resolve<IShootable>("Adapter.Create", obj, typeof(IShootable)));
    }
}
