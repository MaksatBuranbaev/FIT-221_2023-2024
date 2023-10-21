namespace SpaceBattle.Lib;

public class Angle
{
    public int value { get; set; }
    public int dim{get;}

    public Angle(int value, int dim)
    {
        this.value = value % dim;
        this.dim = dim;
    }

    public Angle(int angle)
    {
        this.value = (angle/360 * 8) % 8;
        this.dim = 8;
    }

    public static Angle operator +(Angle ang1, Angle ang2)
    {
        if (ang1.dim != ang2.dim){
            throw new Exception();
        }
        return new Angle(ang1.value + ang2.value, ang1.dim);
    }

    public static bool operator ==(Angle ang1, Angle ang2)
    {
        return (ang1.value == ang2.value && ang1.dim == ang2.dim);
    }

    public static bool operator !=(Angle ang1, Angle ang2)
    {
      return !(ang1 == ang2);
    }

    public override bool Equals(object? obj)
    {
        return obj is Angle && this == (Angle) obj;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}