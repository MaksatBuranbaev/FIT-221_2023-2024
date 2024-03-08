namespace SpaceBattle.Lib;

public class HardStopCommand : ICommand
{
    private readonly ServerThread _st;
    public HardStopCommand(ServerThread st)
    {
        _st = st;
    }
    public void Execute()
    {
        if (_st.Equals(Thread.CurrentThread))
        {
            _st.Stop();
        }
        else
        {
            throw new Exception("Warning");
        }
    }
}
