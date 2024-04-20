using Hwdtech;

namespace SpaceBattle.Lib;

public class InterpretationCommand : ICommand
{
    private readonly IMessage _message;

    public InterpretationCommand(IMessage message)
    {
        _message = message;
    }
    public void Execute()
    {
        var gameId = _message.GameID;
        var cmd = IoC.Resolve<ICommand>("CreateCommandStrategy", _message);
        IoC.Resolve<ICommand>("Queue.Push", gameId, cmd).Execute();
    }
}
