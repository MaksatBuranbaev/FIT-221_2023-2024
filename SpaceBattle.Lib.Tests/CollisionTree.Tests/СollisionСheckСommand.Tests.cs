using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace SpaceBattle.Lib.Tests;

public class СollisionСheckСommandTests
{
    private static Vector? v1;
    private static Vector? v2;
    static СollisionСheckСommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
        "Scopes.Current.Set",
        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CollisionTree", (object[] args) =>
        {
            var tree = new Dictionary<int, object>()
            {
                { 0, new Dictionary<int, object>()},
                { 1, new Dictionary<int, object>()},
                { 2, new Dictionary<int, object>()},
                { 3, new Dictionary<int, object>()}
            };
            ((Dictionary<int, object>)tree[0])[0] = new Dictionary<int, object>();
            ((Dictionary<int, object>)((Dictionary<int, object>)tree[0])[0])[0] = new Dictionary<int, object>();
            ((Dictionary<int, object>)((Dictionary<int, object>)((Dictionary<int, object>)tree[0])[0])[0])[0] = new Dictionary<int, object>();
            return tree;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "DifferenceVector", (object[] args) =>
        {
            var p1 = (Vector)args[0];
            var p2 = (Vector)args[1];
            var v1 = (Vector)args[2];
            var v2 = (Vector)args[3];
            var vector = new List<int>{
                p1.array[0] - p2.array[0],
                p1.array[1] - p2.array[1],
                v1.array[0] - v2.array[0],
                v1.array[1] - v2.array[1],
            };
            return vector;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "UObject1TargetGetProperty", (object[] args) => v1).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "UObject2TargetGetProperty", (object[] args) => v2).Execute();

        var mockCommand = new Mock<Lib.ICommand>();
        mockCommand.Setup(x => x.Execute());

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Event.Collision", (object[] args) => mockCommand.Object).Execute();
    }
    [Fact]
    public void СorrectСollisionСheckСommand()
    {
        v1 = new Vector(new int[] { 1, 1 });
        v2 = new Vector(new int[] { 1, 1 });
        var uOb1 = new Mock<IUObject>();
        var uOb2 = new Mock<IUObject>();

        var ccm = new СollisionСheckСommand(uOb1.Object, uOb2.Object);

        ccm.Execute();

    }

    [Fact]
    public void IncorrectСollisionСheckСommand()
    {
        v1 = new Vector(new int[] { 1, 1 });
        v2 = new Vector(new int[] { 3, 4 });
        var uOb1 = new Mock<IUObject>();
        var uOb2 = new Mock<IUObject>();

        var ccm = new СollisionСheckСommand(uOb1.Object, uOb2.Object);

        ccm.Execute();
    }
}
