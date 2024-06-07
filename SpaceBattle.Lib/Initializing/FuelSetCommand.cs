namespace SpaceBattle.Lib;
using Hwdtech;

public class FuelSetCommand : ICommand
{
    public IList<IUObject>? _uObjects;
    private readonly double _fuelreserve;
    public FuelSetCommand(double fuelreserve) => _fuelreserve = fuelreserve;
    public void Execute()
    {
        _uObjects = IoC.Resolve<List<IUObject>>("Game.UObjects.FuelSet");
        foreach (var uObject in _uObjects)
        {
            uObject.SetProperty("Fuel", _fuelreserve);
        }
    }
}
