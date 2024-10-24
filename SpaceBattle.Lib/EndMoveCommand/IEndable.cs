namespace SpaceBattle.Lib;

public interface IEndable
{
    public InjectCommand cmd { get; }
    public IUObject obj { get; }
    public IEnumerable<string> property { get; }
}
