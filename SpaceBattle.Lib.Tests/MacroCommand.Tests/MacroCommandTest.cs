using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class MacroCommandTests
{
    [Fact]
    public void MacroCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Strategy.MacroCommand",
            (object[] args) =>
            {
                return new MacroCommandStrategy().Run((string[])args);
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.Macro",
            (object[] args) =>
            {
                return new MacroCommand((ICommand[])args);
            }
        ).Execute();

        var moveCommand = new Mock<ICommand>();
        moveCommand.Setup(mc => mc.Execute()).Callback(() => { }).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.Move",
            (object[] args) => moveCommand.Object
        ).Execute();

        var rotateCommand = new Mock<ICommand>();
        moveCommand.Setup(rc => rc.Execute()).Callback(() => { }).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.Rotate",
            (object[] args) => rotateCommand.Object
        ).Execute();

        var commands = new string[] { "Command.Move", "Command.Rotate" };
        var macroCommand = IoC.Resolve<ICommand>("Strategy.MacroCommand", commands);
        macroCommand.Execute();

        moveCommand.Verify(mc => mc.Execute(), Times.Once);
        rotateCommand.Verify(rc => rc.Execute(), Times.Once);
    }
}
