using System.Collections;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class ShipsArrangeIteratorTests
{
    public ShipsArrangeIteratorTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void SuccessfullIterating()
    {
        var positions = new List<Vector>{
            new(new int[]{0,0}),
            new(new int[]{1,1}),
        };

        var uObjects = new List<IUObject>{
            new Mock<IUObject>().Object,
            new Mock<IUObject>().Object,
        };
        
        var shipLocation1 = new ArrayList{
            uObjects[0],
            positions[0],
        };

        var shipLocation2 = new ArrayList{
            uObjects[1],
            positions[1],
        };
        
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Positions.Arrange", (object[] args) => positions).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObjects.Arrange", (object[] args) => uObjects).Execute();

        var iterator = new ShipsArrangeIterator(2);
        var iteratorLocation = new List<ArrayList>();
        foreach(var i in iterator)
        {
            iteratorLocation.Add(i);
        }

        Assert.Equal(shipLocation1, iteratorLocation[0]);
        Assert.Equal(shipLocation2, iteratorLocation[1]);
    }
}