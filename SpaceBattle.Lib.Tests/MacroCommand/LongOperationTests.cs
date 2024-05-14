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

        var queue = new FakeQueue();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue",
            (object[] args) => queue
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.Add",
            (object[] args) =>
            {
                var q = IoC.Resolve<FakeQueue>("Queue");
                var cmd = (Lib.ICommand)args[0];
                var queuePusher = new Mock<Lib.ICommand>();
                queuePusher.Setup(qp => qp.Execute()).Callback(new Action(() => q.Add(cmd)));

                return queuePusher.Object;
            }
        ).Execute();
    }

    [Fact]
    public void СorrectLongOperationTest()
    {
        var mockCommand = new Mock<Lib.ICommand>();

        var name = "Command.Move";
        var mockUObject = new Mock<IUObject>();

        IoC.Resolve<ICommand>("IoC.Register", name, (object[] args) => mockCommand.Object).Execute();
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Operation." + name,
            (object[] args) =>
            {
                var lo = new LongOperation().Run(args);
                return lo;
            }
        ).Execute();

        IoC.Resolve<Lib.ICommand>("Operation." + name, name, mockUObject.Object).Execute();
        mockCommand.VerifyAll();
    }
    [Fact]
    public void InСorrectLongOperationTest()
    {
        var mockCommand = new Mock<Lib.ICommand>();

        var name = "Command.Move.Failed";
        var mockUObject = new Mock<IUObject>();

        Assert.ThrowsAny<Exception>(() => IoC.Resolve<Lib.ICommand>("Operation." + name, name, mockUObject.Object).Execute());
    }
    [Fact]
    public void RepeatCommandTest()
    {

        var mockCommand = new Mock<Lib.ICommand>();
        var repeatCommand = new RepeatCommand(mockCommand.Object);

        repeatCommand.Execute();

        Assert.Equal(mockCommand.Object, IoC.Resolve<FakeQueue>("Queue").Take());
    }
}
