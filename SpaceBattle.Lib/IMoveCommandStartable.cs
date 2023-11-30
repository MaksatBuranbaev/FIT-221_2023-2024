namespace SpaceBattle.Lib;

public interface IQueue
{
    void Add(ICommand cmd);
    ICommand Take();
}

interface IMoveStartable {
    IUObject UObject {get;}

    Vector initialVelocity {get;}

    IQueue Queue { get; }
}

public interface IUObject
{

}