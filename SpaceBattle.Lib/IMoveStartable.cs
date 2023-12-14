namespace SpaceBattle.Lib;

public interface IMoveStartable {
    IUObject UObject {get;}
    Vector initialVelocity {get;}
    IQueue Queue { get; }
}
