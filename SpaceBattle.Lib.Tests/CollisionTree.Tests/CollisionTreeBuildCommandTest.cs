using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class BuildTreeTests
{
    [Fact]
    public void BuildTreeTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var tree = new Dictionary<int, object>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Collision.Tree",
            (object[] args) =>
            {
                return tree;
            }
        ).Execute();

        var reader = new Mock<IReader>();
        var array = new int[][]{
            new int[]{1,1,1},
            new int[]{2,2,2},
            new int[]{1,4,1},
            new int[]{4,5,2},
            new int[]{1,1,2}
        };
        reader.Setup(r => r.Read()).Returns(array);

        new CollisionTreeBuildCommand(reader.Object).Execute();

        var treeBuilt = IoC.Resolve<Dictionary<int, object>>("Collision.Tree");
        Assert.True(IsEqual(new int[] { 1, 2, 4 }, treeBuilt.Keys.ToList()));
        Assert.True(IsEqual(new int[] { 1, 4 }, ((Dictionary<int, object>)treeBuilt[1]).Keys.ToList()));
        Assert.True(IsEqual(new int[] { 1, 2 }, ((Dictionary<int, object>)((Dictionary<int, object>)treeBuilt[1])[1]).Keys.ToList()));
    }

    private static bool IsEqual(int[] ar1, List<int> ar2)
    {
        if (ar1.Length != ar2.Count)
        {
            return false;
        }

        for (var i = 0; i < ar2.Count; i++)
        {
            if (ar1[i] != ar2[i])
            {
                return false;
            }
        }

        return true;
    }
}
