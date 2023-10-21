namespace SpaceBattle.Lib;

public interface IRotateble
{
    public Angle Angle { get; set; }
    public Angle RotationalSpeed { get; }
}

public class RotateCommand : ICommand
{
    private readonly IRotateble rotateble;
    public RotateCommand(IRotateble rotateble)
    {
        this.rotateble = rotateble;
    }
    public void Execute()
    {
        rotateble.Angle = rotateble.Angle + rotateble.RotationalSpeed;
    }
}
