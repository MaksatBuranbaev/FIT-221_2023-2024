using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace SpaceBattle.Lib.Tests;

public class StartMoveCommandTests {
    static StartMoveCommandTests() {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
        "Scopes.Current.Set",
        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.Movable.Create", (object[] args) => 
        {
            var movable = new Mock<IMovable>();
            return movable.Object;
        } ).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.MoveCommand.Create", (object[] args) => 
        {
            var order = new Mock<ICommand>();
            return order.Object;
        } ).Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register", 
            "OrderTargetSetProperty", 
            (object[] args)=> {
                var target = (IUObject)args[0];
                var name = args[1];
                var value = args[2];
                var SetupPropertyCommand = new Mock<ICommand>();
                SetupPropertyCommand.Setup(spc => spc.Execute()).Callback(new Action(() => 
                {
                    var uobj = (IUObject)args[0];
                    var name = (string)args[1];
                    var value = args[2];
                    uobj.SetProperty(name, value);
                }));

                return  SetupPropertyCommand.Object;
            }
        ).Execute();
        
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register", 
            "Queue.Add", 
            (object[] args)=> {
                var q = (IQueue)args[0];
                var val = (ICommand)args[1];

                var QueuePusher = new Mock<ICommand>();
                
                QueuePusher.Setup(qp => qp.Execute()).Callback(new Action(() => q.Add(val)));

                return QueuePusher.Object;
            }
        ).Execute();
    }
    [Fact]
    public void СorrectStartMoveCommand(){
        var moveStartable = new Mock<IMoveStartable>();
        var uobject = new Mock<IUObject>();
        var queue = new Mock<IQueue>();
        var initialVelocity = new Vector(new int[] { 2, 3 });

        moveStartable.SetupGet(ms => ms.Queue).Returns(new FakeQueue()).Verifiable();
        moveStartable.SetupGet(ms=> ms.UObject).Returns(uobject.Object).Verifiable();
        moveStartable.SetupGet(ms => ms.initialVelocity).Returns(initialVelocity).Verifiable();

        var startMoveCommand = new StartMoveCommand(moveStartable.Object);

        startMoveCommand.Execute();

        Assert.NotNull(moveStartable.Object.Queue.Take());
        uobject.Verify();
    }

    [Fact(Timeout = 1000)]
    public void StartMoveCommand_StartableIsNull_Failed() {
        var initialVelocity = new Vector(new int[] { 2, 3 });

        var startMoveCommand = new StartMoveCommand(null!);

        Assert.ThrowsAny<Exception>(() => startMoveCommand.Execute());
    }
}

public class FakeQueue: IQueue
{
    private Lib.ICommand _cmd;
    public void Add(Lib.ICommand cmd)
    {
        _cmd = cmd;
    }

    Lib.ICommand IQueue.Take()
    {
        return _cmd;
    }
}