using Hwdtech;

namespace SpaceBattle.Lib;

public class StopServerCommand : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Остановка сервера...");

        var idThreads = IoC.Resolve<Dictionary<int, object>>("Threads.Dictionary").Keys;

        foreach (var id in idThreads)
        {
            IoC.Resolve<ICommand>("Soft Stop The Thread", id, () =>
                {
                    Console.WriteLine($"\t Остановлен поток {id}");
                }).Execute();
        }

        Console.WriteLine("Все потоки остановлены");
    }
}
