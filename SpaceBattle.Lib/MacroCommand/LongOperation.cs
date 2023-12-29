using Hwdtech;

namespace SpaceBattle.Lib;

public class LongOperation : IStrategy
{
    public object Run(params object[] args)
    {
        var _name = (string)args[0];
        var _obj = (IUObject)args[1];
        var command = IoC.Resolve<ICommand>(_name, _obj);
        var commandsList = new List<ICommand> { command };
        var macroCommand = IoC.Resolve<ICommand>("Command.Macro", commandsList);
        var injectCommand = new InjectCommand(macroCommand);
        var repeatCommand = new RepeatCommand(injectCommand);
        var longOperationCommand = IoC.Resolve<ICommand>("Command.Macro", new List<ICommand> { injectCommand, repeatCommand });
        return longOperationCommand;
    }
}
