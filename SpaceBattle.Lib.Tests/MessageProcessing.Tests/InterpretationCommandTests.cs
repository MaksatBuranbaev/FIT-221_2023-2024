﻿using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class IntepretationCommandTests
{
    [Fact]
    public void InterpretationCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var createCommandStrategy = new CreateCommandStrategy();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "CreateCommandStrategy",
            (object[] args) =>
            {
                return createCommandStrategy.Run(args);
            }).Execute();

        var uobjects = new Dictionary<int, IUObject>();
        uobjects.Add(1, new Mock<IUObject>().Object);
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "UObject.Map",
            (object[] args) =>
            {
                return uobjects;
            }).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "UObject.ById",
            (object[] args) =>
            {
                return (new GetUObjectStrategy()).Run(args);
            }).Execute();

        var setPropertyCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "UObject.SetProperty",
            (object[] args) =>
            {
                return setPropertyCommand.Object;
            }).Execute();

        var startMoveCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.StartMove",
            (object[] args) =>
            {
                return startMoveCommand.Object;
            }).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.Push",
            (object[] args) =>
            {
                var id = (int)args[0];
                var cmd = (ICommand)args[1];
                return new QueuePushCommand(id, cmd);
            }).Execute();

        var queues = new Dictionary<int, Queue<ICommand>>();
        queues.Add(1, new Queue<ICommand>());
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.Map",
            (object[] args) =>
            {
                return queues;
            }).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.QueueById",
            (object[] args) =>
            {
                return (new GetQueueStrategy()).Run(args);
            }).Execute();

        var message = new Mock<IMessage>();
        message.Setup(m => m.TypeCommand).Returns("StartMove");
        message.Setup(m => m.GameID).Returns(1);
        message.Setup(m => m.GameItemId).Returns(1);
        var d = new Dictionary<string, object>();
        d.Add("s", "");
        message.Setup(m => m.Properties).Returns(d);
        (new InterpretationCommand(message.Object)).Execute();

        Assert.True(queues[1].Count == 1);
        setPropertyCommand.Verify(s => s.Execute(), Times.Once);
        Assert.True(queues[1].Dequeue().Equals(startMoveCommand.Object));
    }

    [Fact]
    public void QueueNotInMap()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var queues = new Dictionary<int, Queue<ICommand>>
        {
            { 1, new Queue<ICommand>() }
        };

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Queue.Map",
            (object[] args) =>
            {
                return queues;
            }).Execute();
        Assert.Throws<Exception>((() => new GetQueueStrategy().Run(2)));
    }

    [Fact]
    public void UObjectNotInMap()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var uobjects = new Dictionary<int, IUObject>
        {
            { 1, new Mock<IUObject>().Object }
        };
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "UObject.Map",
            (object[] args) =>
            {
                return uobjects;
            }).Execute();
        Assert.Throws<Exception>((() => new GetUObjectStrategy().Run(3)));
    }
}
