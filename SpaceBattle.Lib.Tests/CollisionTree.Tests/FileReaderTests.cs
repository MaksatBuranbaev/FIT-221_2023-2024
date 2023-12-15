namespace SpaceBattle.Lib.Tests;

public class FileReaderTests
{
    [Fact]
    public void FileReadTest()
    {
        var arrays = new string[]{
            "1 1 1",
            "2 2 2",
            "1 4 1",
            "1 5 2",
            "1 1 2"
        };

        var path = "FileReaderTest.txt";
        File.WriteAllLines(path, arrays);

        var reader = new FileReader(path);
        var arraysFromFile = reader.Read();
        var arraysExpected = new int[][]{
            new int[]{1,1,1},
            new int[]{2,2,2},
            new int[]{1,4,1},
            new int[]{1,5,2},
            new int[]{1,1,2}
        };

        for (var i = 0; i < arraysExpected.Length; i++)
        {
            for (var j = 0; j < arraysExpected[i].Length; j++)
            {
                Assert.Equal(arraysExpected[i][j], arraysFromFile[i][j]);
            }
        }
    }
}
