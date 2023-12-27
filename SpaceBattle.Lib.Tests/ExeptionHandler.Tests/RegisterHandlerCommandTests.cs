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

        var exceptionTree = new Dictionary<Type, object>();
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

        new RegisterHandlerCommand(cmd.Object.GetType(), typeof(Exception), handler.Object).Execute();

        Assert.True(exceptionTree.ContainsKey(cmd.Object.GetType()));
        Assert.True(
            ((Dictionary<Type, object>)exceptionTree[cmd.Object.GetType()]).ContainsKey(typeof(Exception))
            );
        Assert.Equal(
            (IExceptionHandler)((Dictionary<Type, object>)exceptionTree[cmd.Object.GetType()])[typeof(Exception)],
            handler.Object
            );
    }

    [Fact]
    public static void RegisterHandlerForTwoTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var exceptionTree = new Dictionary<Type, object>();
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

        new RegisterHandlerCommand(cmd1.Object.GetType(), typeof(Exception), handler1.Object).Execute();

        Assert.True(exceptionTree.ContainsKey(cmd1.Object.GetType()));
        Assert.True(
            ((Dictionary<Type, object>)exceptionTree[cmd1.Object.GetType()]).ContainsKey(typeof(Exception))
            );
        Assert.Equal(
            (IExceptionHandler)((Dictionary<Type, object>)exceptionTree[cmd1.Object.GetType()])[typeof(Exception)],
            handler1.Object
            );

        var handler2 = new Mock<IExceptionHandler>();
        var cmd2 = new Mock<IInjectableCommand>();

        new RegisterHandlerCommand(cmd2.Object.GetType(), typeof(Exception), handler2.Object).Execute();

        Assert.True(exceptionTree.ContainsKey(cmd2.Object.GetType()));
        Assert.True(
            ((Dictionary<Type, object>)exceptionTree[cmd2.Object.GetType()]).ContainsKey(typeof(Exception))
            );
        Assert.Equal(
            (IExceptionHandler)((Dictionary<Type, object>)exceptionTree[cmd2.Object.GetType()])[typeof(Exception)],
            handler2.Object
            );
    }
}
