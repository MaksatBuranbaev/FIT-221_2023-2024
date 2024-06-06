namespace SpaceBattle.Lib;
public class ArrangeShipCommand: ICommand
{
    public IUObject _uObject;
    private readonly Vector _position;
    public ArrangeShipCommand(IUObject uObject, Vector position)
    {
        _uObject = uObject;
        _position = position;
    }

    public void Execute()
    {
        _uObject.SetProperty("Position", _position);
    }
}