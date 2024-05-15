namespace SpaceBattle.Lib.Tests;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class GameCommandTests
{
    public GameCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void GameCommandSuccessful()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        var mockStrategy = new Mock<IStrategy>();
        mockStrategy.Setup(x => x.Run()).Returns(1000);

        IoC.Resolve<ICommand>("IoC.Register", "Quantum.Get", (object[] args) => mockStrategy.Object.Run(args)).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Exception.Handle",
        (object[] args) =>
        {
            return new DefaultExceptionHandler((Exception)args[1]);
        }).Execute();

        var queue = new Queue<Lib.ICommand>();
        var cmd1 = new Mock<Lib.ICommand>();
        var cmd2 = new Mock<Lib.ICommand>();
        var cmd3 = new Mock<Lib.ICommand>();

        queue.Enqueue(cmd1.Object);
        queue.Enqueue(cmd2.Object);
        queue.Enqueue(cmd3.Object);

        var gameCommand = new GameCommand(scope, queue);
        gameCommand.Execute();

        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
        cmd3.Verify(c => c.Execute(), Times.Once());
        Assert.True(queue.Count == 0);
    }

    [Fact]
    public void GameCommandException()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        var mockStrategy = new Mock<IStrategy>();
        mockStrategy.Setup(x => x.Run()).Returns(1000);

        IoC.Resolve<ICommand>("IoC.Register", "Quantum.Get", (object[] args) => mockStrategy.Object.Run(args)).Execute();

        var handler = new Mock<IExceptionHandler>();
        IoC.Resolve<ICommand>("IoC.Register", "Exception.Handle",
        (object[] args) =>
        {
            return handler.Object;
        }).Execute();

        var queue = new Queue<Lib.ICommand>();
        var cmd1 = new Mock<Lib.ICommand>();
        var cmd2 = new Mock<Lib.ICommand>();
        cmd2.Setup(c => c.Execute()).Throws<Exception>().Verifiable();

        queue.Enqueue(cmd1.Object);
        queue.Enqueue(cmd2.Object);

        var gameCommand = new GameCommand(scope, queue);
        gameCommand.Execute();

        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
    }

    [Fact]
    public void DefaultExceptionHandlerTests()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        var mockStrategy = new Mock<IStrategy>();
        mockStrategy.Setup(x => x.Run()).Returns(1000);

        IoC.Resolve<ICommand>("IoC.Register", "Quantum.Get", (object[] args) => mockStrategy.Object.Run(args)).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Exception.Handle",
        (object[] args) =>
        {
            return new DefaultExceptionHandler((Exception)args[1]);
        }).Execute();

        var queue = new Queue<Lib.ICommand>();
        var cmd1 = new Mock<Lib.ICommand>();
        var cmd2 = new Mock<Lib.ICommand>();
        cmd2.Setup(c => c.Execute()).Throws<Exception>().Verifiable();

        queue.Enqueue(cmd1.Object);
        queue.Enqueue(cmd2.Object);

        var gameCommand = new GameCommand(scope, queue);
        Assert.Throws<Exception>(() => gameCommand.Execute());

        cmd1.Verify(c => c.Execute(), Times.Once());
    }

    [Fact]
    public void EmtyQueue()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        var mockStrategy = new Mock<IStrategy>();
        mockStrategy.Setup(x => x.Run()).Returns(0);

        IoC.Resolve<ICommand>("IoC.Register", "Quantum.Get", (object[] args) => mockStrategy.Object.Run(args)).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Exception.Handle",
        (object[] args) =>
        {
            return new DefaultExceptionHandler((Exception)args[1]);
        }).Execute();

        var queue = new Queue<Lib.ICommand>();
        var gameCommand = new GameCommand(scope, queue);
        gameCommand.Execute();
    }
}
