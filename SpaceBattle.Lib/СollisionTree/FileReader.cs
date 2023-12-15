namespace SpaceBattle.Lib;

public class FileReader : IReader
{
    private readonly string _path;
    public FileReader(string path)
    {
        _path = path;
    }
    public int[][] Read()
    {
        var arrays = File.ReadAllLines(_path).Select(
            line => line.Split(" ").Select(num => int.Parse(num)).ToArray()
        ).ToArray();
        return arrays;
    }
}
