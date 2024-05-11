using Hwdtech;

namespace SpaceBattle.Lib;

public class QueuePushCommand : ICommand
{
    private readonly int _gameId;
    private readonly ICommand _cmd;
    public QueuePushCommand(int gameId, ICommand cmd)
    {
        _gameId = gameId;
        _cmd = cmd;
    }
    public void Execute()
    {
        var queue = IoC.Resolve<Queue<ICommand>>("Queue.QueueById", _gameId);
        queue.Enqueue(_cmd);
    }
}
