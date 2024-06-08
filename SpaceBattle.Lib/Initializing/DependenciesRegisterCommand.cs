namespace SpaceBattle.Lib;
using Hwdtech;

public class DependenciesRegisterCommand : ICommand
{
    private readonly int _gameId;
    public DependenciesRegisterCommand(int gameId)
    {
        _gameId = gameId;
    }

    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "Queue.Push",
            _gameId,
            IoC.Resolve<ICommand>("Command.DependenciesInit")
        ).Execute();
    }
}
