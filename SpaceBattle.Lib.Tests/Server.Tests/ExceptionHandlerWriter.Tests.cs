namespace SpaceBattle.Lib.Tests;

public class ExceptionHandlerWriterTests
{
    [Fact]
    public void CorrectWrite()
    {
        var path = "ExceptionHandlerWriterTest.txt";
        var cmd = "Hard Stop";
        var nameException = "Exception";

        var ehw = new ExceptionHandlerWriter(cmd, path, nameException);
        ehw.Handle();

        var infoFromFile = File.ReadLines(path).ToList().Last();
        var expected = $"{(DateTime.Now).ToString()} {cmd} {nameException}";

        Assert.Equal(expected, infoFromFile);
    }
}
