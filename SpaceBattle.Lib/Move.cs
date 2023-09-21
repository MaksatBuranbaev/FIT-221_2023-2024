namespace SpaceBattle.Lib;

public interface IMovable 
{
	public Vector Position {get; set;}
	public Vector Velocity {get; set;}
}


public class Vector
{
    public int[] array { get; set; }
    public Vector(int[] array)
    {
        this.array = array;
    }

    public static Vector operator +(Vector v1, Vector v2)
    {
        int[] v3 = new int[v1.array.Length];
        for(int i = 0; i < v1.array.Length; i++)
        {
            v3[i] =  v1.array[i] + v2.array[i];
        }
        return new Vector(v3);
    }
}

public class MoveCommand : ICommand
{
    private IMovable movable;

    public MoveCommand(IMovable movable)
    {
        this.movable = movable;
    }
    public void Execute() 
    {
		movable.Position = movable.Position + movable.Velocity;
	}
}