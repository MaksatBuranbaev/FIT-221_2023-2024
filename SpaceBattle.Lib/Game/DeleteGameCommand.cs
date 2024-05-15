using Hwdtech;
namespace SpaceBattle.Lib;

public class DeleteGameCommand : ICommand
{
    private readonly int _gameId;

    public DeleteGameCommand(int gameId)
    {
        _gameId = gameId;
    }

    public void Execute()
    {
        var games = IoC.Resolve<IDictionary<int, IInjectableCommand>>("Games.Map");
        games[_gameId].Inject(
            IoC.Resolve<ICommand>("Command.Empty")
        );

        var scopes = IoC.Resolve<IDictionary<int, object>>("Scope.Map");
        scopes.Remove(_gameId);
    }
}
