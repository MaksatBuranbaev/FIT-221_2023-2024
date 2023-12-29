namespace SpaceBattle.Lib.Tests;
using Moq;
using Hwdtech;
using Hwdtech.Ioc;

public class HandlerExceptionStrategyTest
{
    public HandlerExceptionStrategyTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();


        var defaultHandler = new Mock<IExceptionHandler>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.Handler", (object[] args) => { return defaultHandler.Object; }).Execute();
    }

    [Fact]
    public void SuccesfullFindHandler()
    {
        var tree = new Dictionary<string, IExceptionHandler>();
        var mockHandler = new Mock<IExceptionHandler>();

        var defaultHandler = new Mock<IExceptionHandler>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "DefaultExceptionHandler", (object[] args) => { return defaultHandler.Object; }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Exception.Handler.Tree", (object[] args) => tree).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Exception.Handler.Find",
            (object[] args) => new HandlerExceptionStrategy().Run(args)
        ).Execute();

        tree.Add(mockHandler.ToString(), mockHandler.Object);

        Assert.Equal(mockHandler.Object, IoC.Resolve<IExceptionHandler>("Exception.Handler.Find"));
    }

    [Fact]
    public void FindDefaultHandler()
    {
        var tree = new Dictionary<string, IExceptionHandler>();
        var mockHandler = new Mock<IExceptionHandler>();

        tree.Add(mockHandler.ToString(), mockHandler.Object);

        IoC.Resolve<ICommand>("IoC.Register", "Exception.Handler.Tree", (object[] args) => tree).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Exception.Handler.Find",
            (object[] args) => new HandlerExceptionStrategy().Run(args)
        ).Execute();


        Assert.Equal(mockHandler.Object, IoC.Resolve<IExceptionHandler>("Exception.Handler.Find"));
    }
}