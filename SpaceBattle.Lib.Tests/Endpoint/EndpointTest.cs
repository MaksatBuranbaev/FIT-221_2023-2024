namespace SpaceBattle.Lib.Tests;
using WebHttp;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

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
        
    }
}