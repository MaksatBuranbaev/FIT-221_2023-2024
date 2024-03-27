using Hwdtech;

namespace SpaceBattle.Lib;

public class StopServerCommand : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Остановка сервера...");

        var idThreads = IoC.Resolve<Dictionary<int, object>>("Threads.Dictionary").Keys;

        idThreads.ToList().ForEach(id =>
        {
            var softStopCommand = IoC.Resolve<ICommand>("Soft Stop The Thread", id, () =>
                {
                    IoC.Resolve<ICommand>("Server.Barrier.Command").Execute();
                });
            IoC.Resolve<ICommand>("Server.SendCommand", id, softStopCommand).Execute();
        });

        IoC.Resolve<ICommand>("Server.Barrier.Check").Execute();

        Console.WriteLine("Сервер остановлен");
    }
}
