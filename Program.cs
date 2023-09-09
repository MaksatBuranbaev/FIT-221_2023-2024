Spaceship spaceship = new Spaceship();
spaceship.Move = new Spaceship_Move();
spaceship.Move.Coordinates[0] = 12;
spaceship.Move.Coordinates[1] = 5;
spaceship.Move.Speed[0] = -7;
spaceship.Move.Speed[1] = 3;
spaceship.Move.Move();
Console.WriteLine($"{spaceship.Move.Coordinates[0]}, {spaceship.Move.Coordinates[1]}");
public class Spaceship
    {
        public IMove Move { get; set; }
        public int Health { get; set; }
    }
public class Spaceship_Move: IMove
{
    public int[] Speed { get; set; }
    public int[] Coordinates { get; set; }
    public Spaceship_Move()
    {
        Coordinates = new int [2];
        Speed = new int [2];
    }
    public void Move()
        {
            for (int i = 0; i < Coordinates.Length; i++)
            {
                Coordinates[i] += Speed[i];
            }
        }
}
public interface IMove
    {   
        int[] Speed { get; set; }
        int[] Coordinates { get; set; }
        void Move();
    }
