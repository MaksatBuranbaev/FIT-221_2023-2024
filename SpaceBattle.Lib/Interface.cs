namespace SpaceBattle.Lib;

public interface ICommand 
{
	public void Execute();
}

public interface IMovable 
{
	public int[] Position {get; set;}
	public int[] Velocity {get; set;}
}
