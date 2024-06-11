namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateGameScopeStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var gameId = (int)args[0];
        var parentScope = args[1];
        var quantum = (double)args[2];

        var gameScope = IoC.Resolve<object>("Scopes.New", parentScope);

        IoC.Resolve<IDictionary<int, object>>("Scope.Map").Add(gameId, gameScope);

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            gameScope
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Quantum.Get",
            (object[] args) => (object)quantum
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.Push",
            (object[] args) =>
            {
                var id = (int)args[0];
                var cmd = (ICommand)args[1];
                return new QueuePushCommand(id, cmd);
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.Pop",
            (object[] args) =>
            {
                var id = (int)args[0];
                return new QueuePopStrategy().Run(id);
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "UObject.ById",
            (object[] args) => new GetUObjectStrategy().Run(args)
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "UObject.Delete",
            (object[] args) => new DeleteUObjectCommand((int)args[0])
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            parentScope
        ).Execute();

        return gameScope;
    }
}
