namespace SpaceBattle.Lib;

public class HardStopCommand : ICommand
{
    private readonly ServerThread _st;
        private readonly Action _act;
    public HardStopCommand(ServerThread st, Action act)
    {
        _st = st;
        _act = act;
    }
    public void Execute()
    {
        if (_st.Equals(Thread.CurrentThread))
        {
            _st.Stop();
            _act();
        }
        else
        {
            throw new Exception("Warning");
        }
    }
}
