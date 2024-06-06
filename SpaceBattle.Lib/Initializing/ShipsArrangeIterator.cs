namespace SpaceBattle.Lib;
using Hwdtech;
public class ShipsArrangeIterator
{
    public readonly int _length;
    public IList<Vector> _positions;
    public ShipsArrangeIterator(int length)
    {
        _length = length;
    }
    public IEnumerator<Vector> GetEnumerator()
    {
        for (var i = 0; i < _length; i++)
        {
            _positions = IoC.Resolve<List<Vector>>("Game.Positions");
            yield return _positions[i];
        }
    }
}