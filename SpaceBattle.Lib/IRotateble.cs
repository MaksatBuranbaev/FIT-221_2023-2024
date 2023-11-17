namespace SpaceBattle.Lib;

public interface IRotateble
{
    public Angle Position { get; set; }
    public Angle RotationalSpeed { get; }
}

