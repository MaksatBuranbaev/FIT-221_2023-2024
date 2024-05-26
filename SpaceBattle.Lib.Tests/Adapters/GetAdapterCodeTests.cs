namespace SpaceBattle.Lib.Tests;
using Hwdtech;
using Hwdtech.Ioc;

public class GetAdapterCodeTests
{
    public GetAdapterCodeTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void SuccessfullTest()
    {
        IoC.Resolve<ICommand>("IoC.Register", "GenerateAdapterCode", (object[] args) => new GetAdapterCode().Run(args)).Execute();

        var type = typeof(IMovable);

        var result = IoC.Resolve<string>("GenerateAdapterCode", type);

        var expected = @"
        public class IMovableAdapter : IMovable
        {
            private IUObject _obj;
            public IMovableAdapter(IUObject obj) => _obj = obj;
            public Vector Position { get => (Vector)_obj.GetProperty('Position'); set => _obj.SetProperty('Position', _obj);}
            public Vector Velocity => (Vector)_obj.GetProperty('Velocity');
        }";
        Assert.Equal(expected, result);
    }
}
