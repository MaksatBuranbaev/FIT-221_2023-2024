namespace SpaceBattle.Lib;

public interface IQueue
{
    void Put(ICommand cmd);
    ICommand Get();
}
