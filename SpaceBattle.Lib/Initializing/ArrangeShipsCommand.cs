namespace SpaceBattle.Lib;
using Hwdtech;
public class ArrangeShipsCommand: ICommand
{
    private IUObject? _uObject;
    public void Execute()
    {
        var shipsArrangeIterator = IoC.Resolve<ShipsArrangeIterator>("Game.ShipsArrangeIterator");

        foreach(var position in shipsArrangeIterator)
        {
            _uObject = IoC.Resolve<IUObject>("Game.Arrange.UObject");
            IoC.Resolve<ICommand>("Game.Arrange.Ship", _uObject, position).Execute();
        }
    }
}