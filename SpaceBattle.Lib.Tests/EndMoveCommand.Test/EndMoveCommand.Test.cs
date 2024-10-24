﻿using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class EndCommandTests
{
    public EndCommandTests()
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
            "Command.Inject",
            (object[] args) =>
            {
                var cmd = (IInjectableCommand)args[0];
                var injectCmd = (ICommand)args[1];
                cmd.Inject(injectCmd);
                return cmd;
            }
        ).Execute();
    }

    [Fact]
    public void EndCommandTest()
    {
        var queue = new Queue<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.Pop",
            (object[] args) =>
            {
                var q = (Queue<ICommand>)args[0];
                return q.Dequeue();
            }
        ).Execute();

        var endable = new Mock<IEndable>();

        var cmd = new Mock<ICommand>();
        cmd.Setup(c => c.Execute()).Verifiable();

        var injectCmd = new InjectCommand(cmd.Object);
        queue.Enqueue(injectCmd);
        endable.SetupGet(e => e.cmd).Returns(injectCmd).Verifiable();
        var obj = new Mock<IUObject>();
        endable.SetupGet(e => e.obj).Returns(obj.Object).Verifiable();
        var prop = new List<string>() { "Move" };
        endable.SetupGet(e => e.property).Returns(prop).Verifiable();

        var dict = new Dictionary<string, object>();
        obj.Setup(o => o.SetProperty(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>((key, value) => dict.Add(key, value));
        obj.Setup(o => o.DeleteProperty(It.IsAny<string>())).Callback<string>((string key) => dict.Remove(key));
        obj.Object.SetProperty("Move", new Mock<ICommand>());

        IoC.Resolve<ICommand>("Command.EndMove", endable.Object).Execute();

        IoC.Resolve<ICommand>("Queue.Pop", queue).Execute();

        Assert.Empty(dict);
        Assert.Empty(queue);
        cmd.Verify(c => c.Execute(), Times.Never);
    }

    [Fact]
    public void InjectCommandTest()
    {
        var cmd = new Mock<ICommand>();
        cmd.Setup(c => c.Execute()).Verifiable();

        var injectCommand = new InjectCommand(cmd.Object);
        injectCommand.Inject(IoC.Resolve<ICommand>("Command.Empty"));
        injectCommand.Execute();

        cmd.Verify(c => c.Execute(), Times.Never);
    }
}
