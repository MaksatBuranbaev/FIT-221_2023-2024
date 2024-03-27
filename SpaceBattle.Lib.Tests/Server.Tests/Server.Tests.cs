using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class ServerTests
{
    [Fact]
    public void StartServerTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var castCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Create And Start Thread",
            (object[] args) =>
            {
                return castCommand.Object;
            }).Execute();

        var createBarrierCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Server.Barrier.Create",
            (object[] args) =>
            {
                return createBarrierCommand.Object;
            }).Execute();

        var countThreads = 10;
        (new StartServerCommand(countThreads)).Execute();

        castCommand.Verify(x => x.Execute(), Times.Exactly(countThreads));
        createBarrierCommand.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void StopServerTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var countThreads = 10;
        var threads = new Dictionary<int, object>();
        for (var i = 0; i < countThreads; i++)
        {
            threads.TryAdd(i, "");
        }

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Threads.Dictionary",
            (object[] args) =>
            {
                return threads;
            }
        ).Execute();

        var sendCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Server.SendCommand",
            (object[] args) =>
            {
                sendCommand.Setup(x => x.Execute()).Callback(new Action(() =>
                {
                    ((ICommand)args[1]).Execute();
                }));
                return sendCommand.Object;
            }
        ).Execute();

        var softStopCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Soft Stop The Thread",
            (object[] args) =>
            {
                softStopCommand.Setup(x => x.Execute()).Callback(new Action(() =>
                {
                    ((Action)args[1])();
                }));
                return softStopCommand.Object;
            }
        ).Execute();

        var barrierCommand = new Mock<ICommand>();
        barrierCommand.Setup(x => x.Execute());
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Server.Barrier.Command",
            (object[] args) => barrierCommand.Object
        ).Execute();

        var barrierCheckCommand = new Mock<ICommand>();
        barrierCheckCommand.Setup(x => x.Execute());
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Server.Barrier.Check",
            (object[] args) => barrierCheckCommand.Object
        ).Execute();

        (new StopServerCommand()).Execute();

        sendCommand.Verify(x => x.Execute(), Times.Exactly(countThreads));
        softStopCommand.Verify(x => x.Execute(), Times.Exactly(countThreads));
        barrierCommand.Verify(x => x.Execute(), Times.Exactly(countThreads));
        barrierCheckCommand.Verify(x => x.Execute(), Times.Once);
    }
}
