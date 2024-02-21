namespace SpaceBattle.Lib;

public class ExceptionHandlerWriter : IExceptionHandler
{
    private readonly string _cmd;
    private readonly string _path;
    private readonly string _nameException;

    public ExceptionHandlerWriter(string cmd, string path, string nameException)
    {
        _cmd = cmd;
        _path = path;
        _nameException = nameException;
    }

    public void Handle()
    {
        var data = (DateTime.Now).ToString();
        File.AppendAllText(_path, $"{data} {_cmd} {_nameException}\n");
    }
}
