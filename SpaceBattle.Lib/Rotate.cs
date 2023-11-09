namespace SpaceBattle.Lib;

public interface IRotateble
{
    public Angle Position { get; set; }
    public Angle RotationalSpeed { get; }
}

public class RotateCommand : ICommand
{
    private readonly IRotateble rotatable;
    public RotateCommand(IRotateble rotatable)
    {
        this.rotatable = rotatable;
    }
    public void Execute()
    {
        rotatable.Position += rotatable.RotationalSpeed;
    }
}
