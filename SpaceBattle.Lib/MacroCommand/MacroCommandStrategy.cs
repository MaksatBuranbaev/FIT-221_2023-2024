using Hwdtech;

namespace SpaceBattle.Lib;

public class MacroCommandStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var commands = new List<ICommand>();
        args.ToList().ForEach(cmd =>
        {
            commands.Add(IoC.Resolve<ICommand>((string)cmd));
        });
        return IoC.Resolve<ICommand>("Command.Macro", commands.ToArray());
    }
}
