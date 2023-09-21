namespace SpaceBattle.Lib;

public class Angle
{
    public int value { get; set; } = -1;

    public Angle(int value)
    {
        this.value = value;
    }

    public Angle(){
    }

    public static Angle operator +(Angle ang1, Angle ang2)
    {
        if (ang1.value == -1 || ang2.value == -1){
            throw new System.Exception();
        }
        int angle = (ang1.value + ang2.value) % 360;
        return new Angle(angle);
    }
}

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
