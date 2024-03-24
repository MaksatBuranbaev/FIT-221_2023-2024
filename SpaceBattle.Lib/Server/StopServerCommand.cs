using Hwdtech;

namespace SpaceBattle.Lib;

public class StopServerCommand : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Остановка сервера...");

        var idThreads = IoC.Resolve<Dictionary<int, object>>("Threads.Dictionary").Keys;

        var barrier = new Barrier(idThreads.Count);

        idThreads.ToList().ForEach(id =>
        {
            IoC.Resolve<ICommand>("Soft Stop The Thread", id, () =>
                {
                    barrier.SignalAndWait();
                }).Execute();
        });
        
        Console.WriteLine("Сервер остановлен");
    }
}
