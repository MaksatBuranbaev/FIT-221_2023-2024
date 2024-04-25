namespace SpaceBattle.Lib;
using Hwdtech;

public class GetUObjectStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var uobjectId = (int)args[0];

        IUObject? uobject;

        var uobjects = IoC.Resolve<IDictionary<int, IUObject>>("UObject.Map");

        if (uobjects.TryGetValue(uobjectId, out uobject))
        {
            return uobject;
        }

        throw new Exception();
    }
}
