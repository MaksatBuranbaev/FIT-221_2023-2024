using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class RegisterHandlerCommandTest
{
    [Fact]
    public void RegisterHandlerForOneTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var exceptionTree = new Dictionary<string, IExceptionHandler>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "ExceptionHandler.Tree",
            (object[] args) =>
            {
                return exceptionTree;
            }
        ).Execute();

        var handler = new Mock<IExceptionHandler>();
        var cmd = new Mock<ICommand>();

        new RegisterHandlerCommand(new Type[]{cmd.Object.GetType(), typeof(Exception)}, handler.Object).Execute();

        string key = cmd.Object.GetType().ToString() + typeof(Exception).ToString();
        Assert.True(exceptionTree.ContainsKey(key));
        Assert.Equal(exceptionTree[key], handler.Object);
    }

    [Fact]
    public static void RegisterHandlerForTwoTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var exceptionTree = new Dictionary<string, IExceptionHandler>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "ExceptionHandler.Tree",
            (object[] args) =>
            {
                return exceptionTree;
            }
        ).Execute();

        var handler1 = new Mock<IExceptionHandler>();
        var cmd1 = new Mock<ICommand>();

        new RegisterHandlerCommand(new Type[]{cmd1.Object.GetType(), typeof(Exception)}, handler1.Object).Execute();

        string key1 = cmd1.Object.GetType().ToString() + typeof(Exception).ToString();
        
        var handler2 = new Mock<IExceptionHandler>();
        var cmd2 = new Mock<IInjectableCommand>();

        new RegisterHandlerCommand(new Type[]{cmd2.Object.GetType(), typeof(Exception)}, handler2.Object).Execute();

        string key2 = cmd2.Object.GetType().ToString() + typeof(Exception).ToString();
        
        
        Assert.True(exceptionTree.ContainsKey(key1));
        Assert.Equal(exceptionTree[key1], handler1.Object);

        Assert.True(exceptionTree.ContainsKey(key2));
        Assert.Equal(exceptionTree[key2], handler2.Object);
    }
}
