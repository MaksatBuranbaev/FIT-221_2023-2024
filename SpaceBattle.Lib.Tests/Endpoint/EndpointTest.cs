namespace SpaceBattle.Lib.Tests;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using System.Collections.Concurrent;
using WebHttp;

public class EndpointTest
{
    public EndpointTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    
    [Fact]
    public void  SuccessfulEndpoint()
    {
        var InterpretationCommand = new  Mock<ICommand>();
        var q = new BlockingCollection<Lib.ICommand>();
        var contr = new GameContract
        {
            type = "“fire",
            game_id = "asdfg",
            game_item_id = 548,
        };
        IoC.Resolve<Hwdtech.ICommand>("Command.Interpreted", "IoC.Register", (object[] args) => {
            return InterpretationCommand.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("GetThread", "IoC.Register", (object[] args) => {
            return Thread.CurrentThread;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("SendCommand", "IoC.Register", (object[] args) => {
            q.Add((Lib.ICommand)args[1]);
        }).Execute();
        Assert.True(q.Count == 1);
        var cmd = q.Take();
        cmd.Execute();
        InterpretationCommand.Verify(c => c.Execute(), Times.Once());
    }
}