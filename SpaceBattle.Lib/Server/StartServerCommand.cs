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
        foreach (var id in Enumerable.Range(0, _countThreads))
        {
            IoC.Resolve<ICommand>("Create And Start Thread", id, () =>
                {
                    Console.WriteLine($"\t Запущен поток {id}");
                }).Execute();
        }

        Console.WriteLine($"Запущено {_countThreads} потоков");

    }
}
