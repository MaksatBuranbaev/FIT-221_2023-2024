namespace SpaceBattle.Lib;

public class Vector
{
    public int[] array { get; set; }
    public Vector(int[] array)
    {
        this.array = array;
    }

    public static Vector operator +(Vector v1, Vector v2)
    {
        if (v1.array.Length != v2.array.Length)
        {
            throw new Exception();
        }

        return new Vector(v1.array.Zip(v2.array, (x, y) => x + y).ToArray());
    }

    public static Vector operator -(Vector v1, Vector v2)
    {
        if (v1.array.Length != v2.array.Length)
        {
            throw new Exception();
        }

        return new Vector(v1.array.Zip(v2.array, (x, y) => x - y).ToArray());
    }

    public static bool operator ==(Vector v1, Vector v2)
    {
        return Enumerable.SequenceEqual(v1.array, v2.array);
    }

    public static bool operator !=(Vector v1, Vector v2)
    {
        return !(v1 == v2);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector vector && this == vector;
    }

    public override int GetHashCode()
    {
        return array.GetHashCode();
    }
}
