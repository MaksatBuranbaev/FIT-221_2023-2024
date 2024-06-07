using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class FuelSetCommandTests
{
    public FuelSetCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void SuccessfullFuelSet()
    {
        var obj1 = new Mock<IUObject>();
        var obj2 = new Mock<IUObject>();
        var uObjects = new List<IUObject>{
            obj1.Object,
            obj2.Object,
        };
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObjects.FuelSet", (object[] args) => uObjects).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Ships.FuelSet", (object[] args) => new FuelSetCommand(10)).Execute();
        IoC.Resolve<ICommand>("Game.Ships.FuelSet").Execute();
        obj1.VerifyAll();
        obj2.VerifyAll();
    }
}
