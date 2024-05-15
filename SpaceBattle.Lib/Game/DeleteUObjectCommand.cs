namespace SpaceBattle.Lib;
using Hwdtech;

public class DeleteUObjectCommand : ICommand
{
    private readonly int _uObjectId;

    public DeleteUObjectCommand(int uObjectId)
    {
        _uObjectId = uObjectId;
    }

    public void Execute()
    {
        IoC.Resolve<IDictionary<int, IUObject>>("UObject.Map").Remove(_uObjectId);
    }
}
