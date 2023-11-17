namespace SpaceBattle.Lib;

public interface IRotateble
{
    public Angle Inclination { get; set; }
    public Angle RotationalSpeed { get; }
}
