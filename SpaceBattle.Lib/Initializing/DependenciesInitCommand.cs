namespace SpaceBattle.Lib;
using Hwdtech;

public class DependenciesInitCommand : ICommand
{
    public void Execute()
    {
        var dependencies = IoC.Resolve<IDictionary<string, IStrategy>>("Dependencies.Commands");

        dependencies.ToList().ForEach(d =>
        {
            IoC.Resolve<Hwdtech.ICommand>(
                "IoC.Register",
                "Command." + d.Key,
                (object[] args) => d.Value.Run(args)
            ).Execute();
        });
    }
}
