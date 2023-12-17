using Hwdtech;
namespace SpaceBattle.Lib;

public class СollisionСheckСommand : ICommand
{
    private Dictionary<int, object> _UOb1, _UOb2;

    public СollisionСheckСommand(Dictionary<int, object> UOb1, Dictionary<int, object> UOb2)
    {
        _UOb1 = UOb1;
        _UOb2 = UOb2;
    }

    public void Execute()
    {
        var result = IoC.Resolve<bool>("Game.CheckCollision", _UOb1, _UOb2);

        if (result)
        {
            throw new Exception("Object 1 and object 2 collided");
        }
    }
}