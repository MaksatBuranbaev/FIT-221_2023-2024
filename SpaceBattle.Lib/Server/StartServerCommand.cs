using Hwdtech;

namespace SpaceBattle.Lib;

public class StartServerCommand : ICommand
{
    private readonly int _countThreads;
    public StartServerCommand(int countThreads)
    {
        _countThreads = countThreads;
    }

    public void Execute()
    {
        Console.WriteLine("Запуск сервера...");

        Enumerable.Range(0, _countThreads).ToList().ForEach((int id) =>
        {
            IoC.Resolve<ICommand>("Create And Start Thread", id).Execute();
        });

        IoC.Resolve<ICommand>("Server.Barrier.Create", _countThreads).Execute();

        Console.WriteLine($"Запущено {_countThreads} потоков");
    }
}
