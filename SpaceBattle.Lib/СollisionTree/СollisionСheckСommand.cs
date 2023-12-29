using Hwdtech;
namespace SpaceBattle.Lib;

public class СollisionСheckСommand : ICommand
{
    private readonly IUObject _uOb1, _uOb2;

    public СollisionСheckСommand(IUObject uOb1, IUObject uOb2)
    {
        _uOb1 = uOb1;
        _uOb2 = uOb2;
    }

    public void Execute()
    {
        var result = true;

        var p1 = IoC.Resolve<Vector>("UObject1TargetGetProperty", _uOb1, "Position");
        var p2 = IoC.Resolve<Vector>("UObject2TargetGetProperty", _uOb2, "Position");
        var v1 = IoC.Resolve<Vector>("UObject1TargetGetProperty", _uOb1, "Velocity");
        var v2 = IoC.Resolve<Vector>("UObject2TargetGetProperty", _uOb1, "Velocity");

        var vector = IoC.Resolve<List<int>>("DifferenceVector", p1, p2, v1, v2);
        var node = IoC.Resolve<Dictionary<int, object>>("CollisionTree");

        vector.ForEach(n =>
        {
            try
            {
                node = (Dictionary<int, object>)node[n];
            }

            catch
            {
                result = false;
            }
        });

        if (result)
        {
            IoC.Resolve<ICommand>("Event.Collision", _uOb1, _uOb2).Execute();
        }
    }
}
