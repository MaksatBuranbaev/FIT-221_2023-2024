namespace SpaceBattle.Lib;
using System;

public interface IMovable 
{
	public Vectorn Position {get; set;}
	public Vectorn Velocity {get; }
}


public class Vectorn
{
    public int[] array { get; set; }
    public Vectorn(int[] array)
    {
        this.array = array;
    }

    public static Vectorn operator +(Vectorn v1, Vectorn v2)
    {
        for(int i = 0; i < v1.array.Length; i++)
        {
            v1.array[i] += v2.array[i];
        }
        return v1;
    }

    public static bool operator ==(Vectorn v1, Vectorn v2)
    {
        for(int i = 0; i < v1.array.Length; i++)
        {
            if(v1.array[i] != v2.array[i])
            {
                return false;
            }
        }
        return true;
    }

   public static bool operator !=(Vectorn v1, Vectorn v2)
   {
      return !(v1 == v2);
   }

    public override bool Equals(object? v1)
    {
        return v1 is Vectorn && this == (Vectorn)v1;
    }

    public override int GetHashCode()
    {
        return array.GetHashCode();
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
		movable.Position += movable.Velocity;
	}
}