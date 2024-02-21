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

        for (var id = 0; id < _countThreads; id++)
        {
            IoC.Resolve<ICommand>("Create And Start Thread", id, () =>
                {
                    Console.WriteLine($"\t Запущен поток {id}");
                }).Execute();
        }

        Console.WriteLine($"Запущено {_countThreads} потоков");

    }
}
