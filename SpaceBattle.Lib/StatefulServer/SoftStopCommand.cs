namespace SpaceBattle.Lib;
public class SoftStopCommand : ICommand
{
    private readonly ServerThread _st;
    private readonly Action _strategy;
    private readonly Action _act;
    public SoftStopCommand(ServerThread st, Action act, Action strategy)
    {
        _st = st;
        _act = act;
        _strategy = strategy;
    }
    public void Execute()
    {
        if (_st.Equals(Thread.CurrentThread))
        {
            _st.UpdateBehaviour(_strategy);
            _act();
        }
        else
        {
            throw new Exception("Warning");
        }
    }
}
