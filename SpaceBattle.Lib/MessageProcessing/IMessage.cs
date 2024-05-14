namespace SpaceBattle.Lib;

public interface IMessage
{
    public string TypeCommand { get; }
    public int GameItemId { get; }
    public int GameID { get; }
    public Dictionary<string, object> Properties { get; }
}
