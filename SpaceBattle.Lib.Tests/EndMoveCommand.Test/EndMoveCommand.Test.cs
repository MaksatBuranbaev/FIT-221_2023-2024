﻿using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class EndCommandTests
{
    [Fact]
    public void EndCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.EndMove",
            (object[] args) =>
            {
                var cmd = (IEndable)args[0];
                return new EndMoveCommand(cmd);
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.Empty",
            (object[] args) => new EmptyCommand()
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Inject.Command",
            (object[] args) =>
            {
                var cmd = (IInjectableCommand)args[0];
                var injectCmd = (ICommand)args[1];
                cmd.Inject(injectCmd);
                return cmd;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "UObject.DeleteProperty",
            (object[] args) =>
            {
                var obj = (IUObject)args[0];
                var properties = (List<string>)args[1];
                properties.ForEach(p => obj.DeleteProperty(p));
                return "";
            }
        ).Execute();

        var endable = new Mock<IEndable>();
        var injectCmd = new InjectCommand((new Mock<ICommand>()).Object);
        endable.SetupGet(e => e.cmd).Returns(injectCmd).Verifiable();
        var obj = new Mock<IUObject>();
        endable.SetupGet(e => e.obj).Returns(obj.Object).Verifiable();
        var prop = new List<string>() { "Move" };
        endable.SetupGet(e => e.property).Returns(prop).Verifiable();

        var dict = new Dictionary<string, object>();
        obj.Setup(o => o.SetProperty(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>((key, value) => dict.Add(key, value));
        obj.Setup(o => o.DeleteProperty(It.IsAny<string>())).Callback<string>((string key) => dict.Remove(key));
        obj.Object.SetProperty("Move", "");

        IoC.Resolve<ICommand>("Command.EndMove", endable.Object).Execute();

        Assert.Empty(dict);
    }

    [Fact]
    public void InjectCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.Empty",
            (object[] args) => new EmptyCommand()
        ).Execute();

        var cmd = new Mock<ICommand>();
        cmd.Setup(c => c.Execute()).Verifiable();

        var injectCommand = new InjectCommand(cmd.Object);
        injectCommand.Inject(IoC.Resolve<ICommand>("Command.Empty"));
        injectCommand.Execute();

        cmd.Verify(c => c.Execute(), Times.Never);
    }
}
