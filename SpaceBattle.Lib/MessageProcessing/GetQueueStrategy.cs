namespace SpaceBattle.Lib;
using Hwdtech;

public class GetQueueStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var gameId = (int)args[0];

        Queue<ICommand>? queue;

        var queues = IoC.Resolve<IDictionary<int, Queue<ICommand>>>("Queue.Map");

        if (queues.TryGetValue(gameId, out queue))
        {
            return queue;
        }

        throw new Exception();
    }
}
