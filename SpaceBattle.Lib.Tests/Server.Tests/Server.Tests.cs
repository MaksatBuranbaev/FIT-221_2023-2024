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

        var castCMD = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Create And Start Thread",
            (object[] args) =>
            {
                var cmd = new Mock<ICommand>();
                cmd.Setup(c => c.Execute()).Callback(new Action(() =>
                {
                    ((Action)args[1])();
                    castCMD.Object.Execute();
                }));
                return cmd.Object;
            }).Execute();

        var countThreads = 10;
        (new StartServerCommand(countThreads)).Execute();

        castCMD.Verify(x => x.Execute(), Times.Exactly(countThreads));
    }

    [Fact]
    public void StopServerTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var threads = new Dictionary<int, object>();
        for (var i = 0; i < 10; i++)
        {
            threads.Add(i, "");
        }

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Threads.Dictionary",
            (object[] args) =>
            {
                return threads;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Soft Stop The Thread",
            (object[] args) =>
            {
                var id = (int)args[0];
                var act = (Action)args[1];
                var stopThreadCommand = new Mock<ICommand>();
                stopThreadCommand.Setup(stc => stc.Execute()).Callback(new Action(() =>
                {
                    var t = new Thread(new ThreadStart(act));
                    t.Start();
                    Thread.Sleep(100);
                    threads.Remove(id);
                }));
                return stopThreadCommand.Object;
            }
        ).Execute();

        (new StopServerCommand()).Execute();

        Assert.True(threads.Keys.Count == 0);
    }
}
