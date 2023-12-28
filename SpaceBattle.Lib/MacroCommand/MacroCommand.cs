namespace SpaceBattle.Lib;

public class MacroCommand : ICommand
{
    private readonly ICommand[] _commands;
    public MacroCommand(ICommand[] commands)
    {
        _commands = commands;
    }
    public void Execute()
    {
        _commands.ToList().ForEach(cmd => cmd.Execute());
    }
}
