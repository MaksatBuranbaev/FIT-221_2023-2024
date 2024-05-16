using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class GameTests
{
    public GameTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void CreateGameScopeStrategyTest()
    {
        var scopes = new Dictionary<int, object>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Scope.Map",
            (object[] args) => scopes
        ).Execute();

        var uobject = new Mock<IUObject>();
        var uobjectId = 1;
        var uobjects = new Dictionary<int, IUObject>
        {
            { uobjectId, uobject.Object }
        };
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "UObject.Map",
            (object[] args) => uobjects
        ).Execute();

        var gameId = 99;
        var parentScope = IoC.Resolve<object>("Scopes.Current");
        var quantum = 1.0;

        var commandForPop = new Mock<ICommand>();
        var gameQueue = new Queue<ICommand>();
        gameQueue.Enqueue(commandForPop.Object);
        var queues = new Dictionary<int, Queue<ICommand>>{
            {gameId, gameQueue}
        };
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.QueueById",
            (object[] args) => queues[(int)args[0]]
        ).Execute();

        var scope = (new CreateGameScopeStrategy()).Run(gameId, parentScope, quantum);

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            scope
        ).Execute();

        var uobjectsFromMap = IoC.Resolve<IUObject>("UObject.ById", uobjectId);
        IoC.Resolve<ICommand>("UObject.Delete", uobjectId).Execute();
        var cmdFromQueue = IoC.Resolve<ICommand>("Queue.Pop", gameId);
        var commandForPush = new Mock<ICommand>();
        IoC.Resolve<ICommand>("Queue.Push", gameId, commandForPush.Object).Execute();

        Assert.True(IoC.Resolve<double>("Quantum.Get") == quantum);
        Assert.NotEqual(parentScope, scope);
        Assert.Contains<int>(gameId, scopes.Keys);
        Assert.Equal(cmdFromQueue, commandForPop.Object);
        Assert.Equal(commandForPush.Object, queues[gameId].Dequeue());
        Assert.Equal(uobject.Object, uobjectsFromMap);
        Assert.Empty(uobjects);
    }

    [Fact]
    public void DeleteUObjectCommandTest()
    {
        var uobjects = new Dictionary<int, IUObject>
        {
            { 1, new Mock<IUObject>().Object }
        };
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "UObject.Map",
            (object[] args) =>
            uobjects
        ).Execute();

        new DeleteUObjectCommand(1).Execute();

        Assert.DoesNotContain<int>(1, uobjects.Keys);
    }

    [Fact]
    public void DeleteGameCommandTest()
    {
        var injectCommand = new Mock<IInjectableCommand>();
        var games = new Dictionary<int, IInjectableCommand>{
            { 993, injectCommand.Object }
        };
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Games.Map",
            (object[] args) => games
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.Empty",
            (object[] args) =>
           new Mock<ICommand>().Object).Execute();

        var scopes = new Dictionary<int, object>{
            { 993, ""}
        };
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Scope.Map",
            (object[] args) => scopes
        ).Execute();

        new DeleteGameCommand(993).Execute();

        Assert.DoesNotContain<int>(993, scopes.Keys);
        injectCommand.Verify(i => i.Inject(It.IsAny<ICommand>()), Times.Once);
    }

    [Fact]
    public void QueuePopStrategyTest()
    {
        var gameId = 32;

        var command = new Mock<ICommand>();
        var gameQueue = new Queue<ICommand>();
        gameQueue.Enqueue(command.Object);
        var queues = new Dictionary<int, Queue<ICommand>>{
            {gameId, gameQueue}
        };
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.QueueById",
            (object[] args) => queues[(int)args[0]]
        ).Execute();

        var queueCommand = new QueuePopStrategy().Run(gameId);

        Assert.Equal(command.Object, queueCommand);
    }

    [Fact]
    public void CreateGameStrategyTest()
    {
        var gameId = 999;
        var parentScope = IoC.Resolve<object>("Scopes.Root");
        var quantum = 1.0;

        var createGameScopeStrategy = new Mock<IStrategy>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Scope.New",
            (object[] args) => createGameScopeStrategy.Object.Run(args)
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.New",
            (object[] args) => new Queue<ICommand>()
        ).Execute();

        var gameCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.Game",
            (object[] args) => gameCommand.Object
        ).Execute();

        var macroCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.Macro",
            (object[] args) => macroCommand.Object
        ).Execute();

        var injectCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.Inject",
            (object[] args) => injectCommand.Object
        ).Execute();

        var repeatCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.Repeat",
            (object[] args) => repeatCommand.Object
        ).Execute();

        var games = new Dictionary<int, ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Map",
            (object[] args) => games
        ).Execute();

        var queues = new Dictionary<int, Queue<ICommand>>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.Map",
            (object[] args) =>
            {
                return queues;
            }).Execute();

        var res = new CreateGameCommandStrategy().Run(gameId, parentScope, quantum);

        var gameMap = IoC.Resolve<IDictionary<int, ICommand>>("Game.Map");
        Assert.Equal(gameMap[gameId], injectCommand.Object);
        createGameScopeStrategy.Verify(c => c.Run(It.IsAny<object[]>()), Times.Once);
        Assert.Contains<int>(gameId, queues.Keys);
    }
}
