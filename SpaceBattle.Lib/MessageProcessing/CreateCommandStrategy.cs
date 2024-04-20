using Hwdtech;

namespace SpaceBattle.Lib;

public class CreateCommandStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var message = (IMessage)args[0];

        var commandName = message.TypeCommand;
        var gameItemId = message.GameItemId;
        var prop = message.Properties;

        var uobj = IoC.Resolve<IUObject>("UObject.ById", gameItemId);
        prop.ToList().ForEach(p => IoC.Resolve<ICommand>("UObject.SetProperty", uobj, p.Key, p.Value).Execute());

        return IoC.Resolve<ICommand>("Command." + commandName, uobj);
    }
}
