namespace SpaceBattle.Lib;
using Hwdtech;
public class ShipsArrangeCommand : ICommand
{
    public void Execute()
    {
        var shipsArrangeIterator = IoC.Resolve<ShipsArrangeIterator>("Game.ShipsArrangeIterator");

        foreach (var ship in shipsArrangeIterator)
        {
            IoC.Resolve<ICommand>("Game.Ship.Arrange", ship[0]!, ship[1]!).Execute();
        }
    }
}
