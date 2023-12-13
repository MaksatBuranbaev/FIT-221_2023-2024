using System.Collections;
using Moq;
namespace SpaceBattle.Lib.Tests;
using Hwdtech;

public class StartMoveCommandTests {
    static StartMoveCommandTests() {
        IoC.Resolve<int>("IoC.Register", "OrderTargetSetProperty", () => 
        {
            var dict = new Dictionary<string, object>();
        } );
        IoC.Resolve<int>("IoC.Register", "Velocity", (object[] args) => 
        {
            IoC.Resolve<IDictionary>("OrderTargetSetProperty").Add(args[0], args[1]); 
        });
        IoC.Resolve<int>("IoC.Register", "ChangeVelocity", (object[] args) => {args[0] = args[1];} );
        IoC.Resolve<int>("IoC.Register", "Commands.Movable.Create", (object[] args) => {return new Mock<IMovable>(args[0]);} );
        IoC.Resolve<int>("IoC.Register", "Commands.MoveCommand.Create", (object[] args) => {return new Mock<MoveCommand>(args[0]);} );
    }

    [Fact]
    public void StartMoveCommand_Success(){
        // Init test dependencies
        var MoveCmdStartable = new Mock<IMoveCommandStartable>();
        var UObjectMock = new Mock<IUObject>();
        List<int> Velocity = new List<int>{ 42 };

        UObjectMock.SetupSet(uo => uo["velocity"] = Velocity).Verifiable();

        MoveCmdStartable.SetupGet(mcs => mcs.Queue).Returns(new FakeQueue<ICommand>());
        MoveCmdStartable.SetupGet(mcs => mcs.Velocity).Returns(Velocity);
        MoveCmdStartable.SetupGet(mcs => mcs.UObject).Returns(UObjectMock.Object);

        // Create StartMoveCommand
        var smc = new StartMoveCommand(MoveCmdStartable.Object);

        // Action
        smc.Run();

        // Assertation
        Assert.NotNull(MoveCmdStartable.Object.Queue.Pop());
        UObjectMock.Verify();
    }

    [Fact(Timeout = 1000)]
    public void StartMoveCommand_StartableIsNull_Failed() {
        // Init test dependencies
        List<int> Velocity = new List<int>{ 42 };

        // Create StartMoveCommand
        var smc = new StartMoveCommand(null!);


        // Assertation
        Assert.ThrowsAny<Exception>(() => smc.Run());
    }
}



class SetupPropertyStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var target = (IUObject)argv[0];
        var value = argv[2];

        var SetupPropertyCommand = new Mock<ICommand>();
        SetupPropertyCommand.Setup(spc => spc.Run()).Callback(new Action(() => target["velocity"] = value ));

        return  SetupPropertyCommand.Object;
    }
}

class MovableAdapterInjectStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var Mover = new Mock<IMovable>();

        return Mover.Object;
    }
}

class CommandInjectStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var cmd = new Mock<ICommand>();
        return cmd.Object;
    }
}

class QueuePushStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var q = (IQueue<ICommand>)argv[0];
        var val = (ICommand)argv[1];

        var QueuePusher = new Mock<ICommand>();
        
        QueuePusher.Setup(qp => qp.Run()).Callback(new Action(() => q.Push(val)));

        return QueuePusher.Object;
    }
}

class FakeQueue<T> : IQueue<T>
{
    private T? cmd = default(T);
    public T Pop()
    {
        var returnValue = cmd;
        cmd = default(T)!;

        return returnValue!;
    }

    public void Push(T elem)
    {
        this.cmd = elem;
    }
}