namespace SpaceBattle.Lib;

public interface MoveCommandStartable
{

}

public interface IQueue
{
    void Add(ICommand cmd);
    ICommand Take();
}