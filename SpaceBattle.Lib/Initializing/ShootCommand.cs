namespace SpaceBattle.Lib;
using Hwdtech;

public class ShootCommand : ICommand
{
    private readonly IShootable _shotable;

    public ShootCommand(IShootable shotable)
    {
        _shotable = shotable;
    }

    public void Execute()
    {
        var projectile = IoC.Resolve<IMovable>("Adapter.Create.Movable", _shotable);
        IoC.Resolve<ICommand>("Command.StartMove", projectile).Execute();
    }
}
