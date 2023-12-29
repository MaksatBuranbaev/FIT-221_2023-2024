namespace SpaceBattle.Lib.Tests;
using Moq;
using Hwdtech;
using Hwdtech.Ioc;

public class LongOperationTests 
{
    public LongOperationTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Queue",
            (object[] args) => new Mock<IQueue>()

        ).Execute();
    }

    [Fact]
    public void LongOperation()
    {
        var mockCommand = new Mock<Lib.ICommand>();
        mockCommand.Setup(x => x.Execute()).Verifiable();

        var name = "Command.Movement";
        var mockUObject = new Mock<IUObject>();

        IoC.Resolve<ICommand>("IoC.Register", name, (object[] args) => mockCommand.Object).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Command.Macro", (object[] args) => mockCommand.Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Inject.Create",
            (object[] args) =>
            {
                var inject = new InjectCommand((Lib.ICommand)args[0]);
                return inject;
            }
        ).Execute();
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Command.Repeat",
            (object[] args) =>
            {
                var queuePusher = new Mock<Lib.ICommand>();
                queuePusher.Setup(qp => qp.Execute()).Callback(new Action(() => IoC.Resolve<IQueue>("Queue").Add((Lib.ICommand)args[0])));

                return queuePusher.Object;
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Command.LongOperation",
            (object[] args) => 
            {
                var mC = (Lib.ICommand) args[0];
                var cL = (List<Lib.ICommand>) args[1];
                var injectCommand = IoC.Resolve<Lib.ICommand>("Inject.Create", mC);
                var repeatCommand = IoC.Resolve<Lib.ICommand>("Command.Repeat", injectCommand);
                cL.Add(repeatCommand);
                return injectCommand;
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Operation." + name,
            (object[] args) => 
            {
                var loc = new LongOperation();
                var lo = loc.Run(args);
                return lo;
            }
        ).Execute();

        IoC.Resolve<Lib.ICommand>("Operation." + name, name, mockUObject.Object).Execute();
        mockCommand.VerifyAll();
    }
}