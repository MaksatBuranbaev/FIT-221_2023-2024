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
        for (int i = 0; i < v1.array.Length; i++)
        {
            if (v1.array[i] != v2.array[i])
            {
                return false;
            }
        }
        return true;
    }

    public static bool operator !=(Vector v1, Vector v2)
    {
        return !(v1 == v2);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector && this == (Vector)obj;
    }

    public override int GetHashCode()
    {
        return array.GetHashCode();
    }
}