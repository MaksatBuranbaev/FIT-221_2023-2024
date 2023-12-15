using Hwdtech;

namespace SpaceBattle.Lib;

public class CollisionTreeBuildCommand : ICommand
{
    private readonly IReader _reader;
    public CollisionTreeBuildCommand(IReader reader)
    {
        _reader = reader;
    }
    public void Execute()
    {
        var array = _reader.Read();
        array.ToList().ForEach(vector =>
        {
            var node = IoC.Resolve<Dictionary<int, object>>("Collision.Tree");
            vector.ToList().ForEach(n =>
            {
                node[n] = node.GetValueOrDefault(n, new Dictionary<int, object>());
                node = (Dictionary<int, object>)node[n];
            });
        });

    }
}
