namespace SpaceBattle.Lib;
public class ShipArrangeCommand: ICommand
{
    public IUObject _uObject;
    private readonly Vector _position;
    public ShipArrangeCommand(IUObject uObject, Vector position)
    {
        _uObject = uObject;
        _position = position;
    }

    public void Execute()
    {
        _uObject.SetProperty("Position", _position);
    }
}