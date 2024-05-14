using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace SpaceBattle.Lib.Tests;

public class StartMoveCommandTests
{
    static StartMoveCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
        "Scopes.Current.Set",
        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.Movable.Create", (object[] args) =>
        {
            var movable = new Mock<IMovable>();
            return movable.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.MoveCommand.Create", (object[] args) =>
        {
            var order = new Mock<ICommand>();
            return order.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "OrderTargetSetProperty",
            (object[] args) =>
            {
                var uobj = (IUObject)args[0];
                var name = (string)args[1];
                var value = args[2];
                var setupPropertyCommand = new Mock<ICommand>();
                setupPropertyCommand.Setup(spc => spc.Execute()).Callback(new Action(() =>
                {
                    uobj.SetProperty(name, value);
                }));

                return setupPropertyCommand.Object;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.Add",
            (object[] args) =>
            {
                var q = (IQueue)args[0];
                var value = (ICommand)args[1];
                var queuePusher = new Mock<ICommand>();
                queuePusher.Setup(qp => qp.Execute()).Callback(new Action(() => q.Add(value)));

                return queuePusher.Object;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Inject.Create",
            (object[] args) =>
            {
                var inject = new InjectCommand((ICommand)args[0]);
                return inject;
            }
        ).Execute();
    }
    [Fact]
    public void СorrectStartMoveCommand()
    {
        var moveStartable = new Mock<IMoveStartable>();
        var uobject = new Mock<IUObject>();
        var queue = new Mock<IQueue>();
        var initialVelocity = new Vector(new int[] { 2, 3 });

        moveStartable.SetupGet(ms => ms.Queue).Returns(new FakeQueue()).Verifiable();
        moveStartable.SetupGet(ms => ms.UObject).Returns(uobject.Object).Verifiable();
        moveStartable.SetupGet(ms => ms.initialVelocity).Returns(initialVelocity).Verifiable();

        var startMoveCommand = new StartMoveCommand(moveStartable.Object);

        startMoveCommand.Execute();

        Assert.NotNull(moveStartable.Object.Queue.Take());
        uobject.Verify();
    }

    [Fact]
    public void StartMoveCommand_StartableIsNull_Failed()
    {
        var initialVelocity = new Vector(new int[] { 2, 3 });

        var startMoveCommand = new StartMoveCommand(null!);

        Assert.ThrowsAny<Exception>(() => startMoveCommand.Execute());
    }

    [Fact]
    public void InjectCommandTest()
    {
        var cmd = new Mock<ICommand>();
        cmd.Setup(c => c.Execute()).Verifiable();

        var injectCommand = new InjectCommand(cmd.Object);
        injectCommand.Inject(new Mock<ICommand>().Object);
        injectCommand.Execute();

        cmd.Verify(c => c.Execute(), Times.Never);
    }
}

public class FakeQueue : IQueue
{
    private ICommand _cmd = new Mock<ICommand>().Object;
    public void Add(ICommand cmd)
    {
        _cmd = cmd;
    }

    ICommand IQueue.Take()
    {
        return _cmd;
    }
}
