namespace SpaceBattle.Lib;

public class CreateAndStartThread : ICommand
{
    private readonly ServerThread _st;
    private readonly Action _act;
    public CreateAndStartThread(ServerThread st, Action act)
    {
        _st = st;
        _act = act;
    }
    public void Execute()
    {
        _st.Start();
        _act();
    }
}
