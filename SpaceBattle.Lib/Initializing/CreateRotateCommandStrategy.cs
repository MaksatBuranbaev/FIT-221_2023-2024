namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateRotateCommandStrategy: IStrategy
{
    public object Run(params object[] args)
    {
        var obj = (IUObject)args[0];
        return new RotateCommand(IoC.Resolve<IRotateble>("Adapter.Create", obj, typeof(IRotateble)));
    }
}