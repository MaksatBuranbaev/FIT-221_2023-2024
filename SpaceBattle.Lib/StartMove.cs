namespace SpaceBattle.Lib;

public interface IQueue
{
    void Add(ICommand cmd);
    ICommand Take();
}

public class StartCommand()
{
    
}