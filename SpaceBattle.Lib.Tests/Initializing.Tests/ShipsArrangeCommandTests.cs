using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class ShipsArrangeCommandTests
{
    public ShipsArrangeCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void SuccessfullShipsArrange()
    {
        var mockShipsIterator = new Mock<ShipsArrangeIterator>(2);
        var mockCommand = new Mock<ICommand>();

        var positions = new List<Vector>{
            new(new int[]{0,0}),
            new(new int[]{0,0}),
        };
        var uObjects = new List<IUObject>{
            new Mock<IUObject>().Object,
            new Mock<IUObject>().Object,
        };

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.ShipsArrangeIterator", (object[] args) => mockShipsIterator.Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Positions.Arrange", (object[] args) => positions).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObjects.Arrange", (object[] args) => uObjects).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Ship.Arrange", (object[] args) => mockCommand.Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Ships.Arrange", (object[] args) => new ShipsArrangeCommand()).Execute();

        IoC.Resolve<ICommand>("Game.Ships.Arrange").Execute();
        mockShipsIterator.VerifyAll();
        mockCommand.VerifyAll();
    }
}
