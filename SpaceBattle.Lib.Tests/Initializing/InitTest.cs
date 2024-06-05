using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class InitTest
{
    [Fact]
    public void DependenciesRegisterCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var depInitСommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.DependenciesInit",
            (object[] args) =>
            {
                return depInitСommand.Object;
            }
        ).Execute();

        var pCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.Push",
            (object[] args) =>
            {
                ((ICommand)args[1]).Execute();
                return pCommand.Object;
            }
        ).Execute();

        var gameId = 3;
        new DependenciesRegisterCommand(gameId).Execute();

        pCommand.Verify(p => p.Execute(), Times.Once);
        depInitСommand.Verify(d => d.Execute(), Times.Once);
    }

    [Fact]
    public void DependenciesInitCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var MockStrategy = new Mock<IStrategy>();
        var dependencies = new Dictionary<string, IStrategy>(){
            {"Test", MockStrategy.Object },
            {"StartMove", new CreateStartMovementCommandStrategy() },
            {"Rotate", new CreateRotateCommandStrategy() },
            {"Shoot", new CreateShootCommandStrategy() },
        };
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Dependencies.Commands",
            (object[] args) =>
            {
                return dependencies;
            }
        ).Execute();

        new DependenciesInitCommand().Execute();

        IoC.Resolve<IStrategy>("Command.Test");
        IoC.Resolve<IStrategy>("Command.Create", "Test", new object());

        var IUObject = new Mock<IUObject>();

        var mocks = new Dictionary<Type, object>(){
            {typeof(IRotateble), new Mock<IRotateble>().Object},
            {typeof(IMoveStartable), new Mock<IMoveStartable>().Object},
            {typeof(IShootable), new Mock<IShootable>().Object},
        };
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Adapter.Create",
            (object[] args) =>
            {
                return mocks[(Type)args[1]];
            }
        ).Execute();

        var rotateCommand = IoC.Resolve<ICommand>("Command.Rotate", IUObject.Object);
        var startMoveCommand = IoC.Resolve<ICommand>("Command.StartMove", IUObject.Object);
        var shootCommand = IoC.Resolve<ICommand>("Command.Shoot", IUObject.Object);

        Assert.IsType<RotateCommand>(rotateCommand);
        Assert.IsType<StartMoveCommand>(startMoveCommand);
        Assert.IsType<ShootCommand>(shootCommand);
        MockStrategy.Verify(s => s.Run(), Times.Once);
    }

    [Fact]
    public void ShootCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Adapter.Create.Movable",
            (object[] args) =>
            {
                return new Mock<IMovable>().Object;
            }
        ).Execute();

        var startMoveCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.StartMove",
            (object[] args) =>
            {
                return startMoveCommand.Object;
            }
        ).Execute();

        var shootable = new Mock<IShootable>().Object;
        new ShootCommand(shootable).Execute();
        startMoveCommand.Verify(s => s.Execute(), Times.Once);
    }
}
