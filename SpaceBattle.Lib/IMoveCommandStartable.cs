namespace SpaceBattle.Lib;

public interface IQueue
{
    void Add(ICommand cmd);
    ICommand Take();
}

public interface IMoveStartable {
    IUObject UObject {get;}

    Vector initialVelocity {get;}

    IQueue Queue { get; }
}

public interface IUObject
{
    public object UObject { get; set;}
}