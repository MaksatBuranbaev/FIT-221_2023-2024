namespace SpaceBattle.Lib;
using System.Collections;
using Hwdtech;
public class ShipsArrangeIterator
{
    public readonly int _length;
    public IList<Vector>? _positions;
    public IList<IUObject>? _uObjects;
    public ShipsArrangeIterator(int length)
    {
        _length = length;
    }
    public IEnumerator<ArrayList> GetEnumerator()
    {
        for (var i = 0; i < _length; i++)
        {
            _positions = IoC.Resolve<List<Vector>>("Game.Positions.Arrange");
            _uObjects = IoC.Resolve<List<IUObject>>("Game.UObjects.Arrange");
            yield return new ArrayList(){_uObjects[i], _positions[i]};
        }
    }
}