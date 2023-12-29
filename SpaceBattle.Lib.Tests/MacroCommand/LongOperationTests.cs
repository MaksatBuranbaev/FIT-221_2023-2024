namespace SpaceBattle.Lib.Tests;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class LongOperationTests
{
    public LongOperationTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void LongOperation()
    {
        var mockCommand = new Mock<Lib.ICommand>();
        mockCommand.Setup(x => x.Execute()).Verifiable();

        var name = "Command.Movement";
        var mockUObject = new Mock<IUObject>();

        IoC.Resolve<ICommand>("IoC.Register", name, (object[] args) => mockCommand.Object).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Command.Macro", (object[] args) => mockCommand.Object).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Operation." + name,
            (object[] args) =>
            {
                var loc = new LongOperation();
                var lo = loc.Run(args);
                return lo;
            }
        ).Execute();

        IoC.Resolve<Lib.ICommand>("Operation." + name, name, mockUObject.Object).Execute();
        mockCommand.VerifyAll();
    }

    [Fact]
    public void RepeatCommandTest()
    {

        var mockCommand = new Mock<Lib.ICommand>();

        var queue = new FakeQueue();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.Add",
            (object[] args) =>
            {
                var q = queue;
                var cmd = (Lib.ICommand)args[0];
                var queuePusher = new Mock<Lib.ICommand>();
                queuePusher.Setup(qp => qp.Execute()).Callback(new Action(() => q.Add(cmd)));

                return queuePusher.Object;
            }
        ).Execute();

        var repeatCommand = new RepeatCommand(mockCommand.Object);

        repeatCommand.Execute();

        Assert.Equal(mockCommand.Object, queue.Take());
    }
}
