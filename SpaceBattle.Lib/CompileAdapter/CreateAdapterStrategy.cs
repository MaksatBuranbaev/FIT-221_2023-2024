namespace SpaceBattle.Lib;
using System.Reflection;
using Hwdtech;

public class CreateAdapterStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var uObject = (IUObject)args[0];
        var uobjectType = uObject.GetType();
        var targetType = (Type)args[1];

        var map = IoC.Resolve<IDictionary<KeyValuePair<Type, Type>, Assembly>>("Adapter.Map");
        var pair = new KeyValuePair<Type, Type>(uobjectType, targetType);

        if (!map.ContainsKey(pair))
        {
            IoC.Resolve<ICommand>("Adapter.Compile", uobjectType, targetType).Execute();
        }

        return IoC.Resolve<object>("Adapter.Find", uObject, targetType);
    }
}
