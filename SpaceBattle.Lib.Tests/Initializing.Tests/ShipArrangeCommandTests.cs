using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class ShipsArrangeCommandTest
{
    public ShipsArrangeCommandTest()
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
        var mockUObject = new Mock<IUObject>();
        var mockVector = new Mock<Vector>(new int[]{1,1});
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Ship.Arrange", (object[] args) => new ShipArrangeCommand(mockUObject.Object, mockVector.Object)).Execute();

        IoC.Resolve<ICommand>("Game.Ship.Arrange").Execute();
        mockUObject.VerifyAll();
        mockVector.VerifyAll();
    }
}