using Hwdtech;

namespace SpaceBattle.Lib;

class Program
{
    static void Main(string[] args)
    {
        var countThreads = int.Parse(args[0]);

        IoC.Resolve<ICommand>("Server.StartServer", countThreads).Execute();

        Console.WriteLine("Для остановки сервера нажмите любую клавишу...");
        Console.ReadKey();

        IoC.Resolve<ICommand>("Server.StopServer").Execute();
    }
}
