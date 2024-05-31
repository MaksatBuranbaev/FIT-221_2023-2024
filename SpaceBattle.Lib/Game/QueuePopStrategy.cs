using Hwdtech;

namespace SpaceBattle.Lib;

public class QueuePopStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var queue = IoC.Resolve<Queue<ICommand>>("Queue.QueueById", (int)args[0]);
        return queue.Dequeue();
    }
}
