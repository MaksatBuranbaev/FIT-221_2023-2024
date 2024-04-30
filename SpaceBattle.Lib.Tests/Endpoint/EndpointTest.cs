namespace SpaceBattle.Lib.Tests;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using System.Collections.Concurrent;
using SpaceBattle.Server;

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
        var InterpretationCommand = new  Mock<Lib.ICommand>();
        var q = new BlockingCollection<Lib.ICommand>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Command.Interpreted", (object[] args) => {
            return InterpretationCommand.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetThread", (object[] args) => {
            return Thread.CurrentThread;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommand", (object[] args) => {
             var sendCommand = new Mock<Lib.ICommand>();
                sendCommand.Setup(sc => sc.Execute()).Callback(new Action(() =>
                {
                    q.Add((Lib.ICommand)args[1]);
                }));
                return sendCommand.Object;
        }).Execute();
        var contr = new GameContract
        {
            type = "“fire",
            game_id = "asdfg",
            game_item_id = 548,
            properties = new List<int>(){1, 2, 3}
        };
        var endpoint = new Endpoint(contr);
        endpoint.Execute();
        Assert.True(q.Count == 1);
        var cmd = q.Take();
        cmd.Execute();
        InterpretationCommand.Verify(c => c.Execute(), Times.Once());
    }
}