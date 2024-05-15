namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateGameCommandStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var gameId = (int)args[0];
        var parentScope = args[1];
        var quantum = (double)args[2];

        var gameScope = IoC.Resolve<object>("Scope.New", gameId, parentScope, quantum);
        var gameQueue = IoC.Resolve<object>("Queue.New");
        var gameCommand = IoC.Resolve<ICommand>("Command.Game", gameQueue, gameScope);

        var commandsList = new List<ICommand> { gameCommand };
        var macroCommand = IoC.Resolve<ICommand>("Command.Macro", commandsList);
        var injectCommand = IoC.Resolve<ICommand>("Command.Inject", macroCommand);
        var repeatCommand = IoC.Resolve<ICommand>("Command.Repeat", injectCommand);
        commandsList.Add(repeatCommand);

        IoC.Resolve<IDictionary<int, ICommand>>("Game.Map").Add(gameId, injectCommand);

        return injectCommand;
    }
}
