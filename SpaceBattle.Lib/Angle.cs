namespace SpaceBattle.Lib;

public class Angle
{
    public int Value { get; set; }
    public int Dim { get; }

    public Angle(int Value, int Dim)
    {
        this.Value = Value % Dim;
        this.Dim = Dim;
    }

    public Angle(int Value)
    {
        this.Value = Value;
        Dim = 8;
    }

    public static Angle operator +(Angle ang1, Angle ang2)
    {
        return new Angle(ang1.Value + ang2.Value, ang1.Dim);
    }

    public static bool operator ==(Angle ang1, Angle ang2)
    {
        return (ang1.Value == ang2.Value && ang1.Dim == ang2.Dim);
    }

    public static bool operator !=(Angle ang1, Angle ang2)
    {
        return !(ang1 == ang2);
    }

    public override bool Equals(object? obj)
    {
        return obj is Angle angle && this == angle;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}
