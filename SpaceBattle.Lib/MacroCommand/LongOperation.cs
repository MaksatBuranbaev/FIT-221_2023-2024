using Hwdtech;

namespace SpaceBattle.Lib;

public class LongOperation : IStrategy
{
    public object Run(params object[] args)
    {
        var _name = (string)args[0];
        var _obj = (IUObject)args[1];
        var command = IoC.Resolve<ICommand>(_name, _obj);
        var injectCommand = new InjectCommand(command);
        var repeatCommand = new RepeatCommand();
        var longOperationCommand = IoC.Resolve<ICommand>("Command.Macro", new List<ICommand> { injectCommand, repeatCommand });
        repeatCommand.Add(longOperationCommand);
        return longOperationCommand;
    }
}
