namespace SpaceBattle.Lib.Tests;

public class AngleTest
{
    [Theory]
    [InlineData(1, 4, 3, 4, 0, 4)]
    public void AngleSumW(int v1, int d1, int v2, int d2, int v3, int d3)
    {
        var act = new Angle(v1, d1) + new Angle(v2, d2);

        Assert.Equal(act, new Angle(v3, d3));
    }

    [Theory]
    [InlineData(1, 4, 3, 4)]
    public void AngleCompF(int v1, int d1, int v2, int d2)
    {

        Assert.False(new Angle(v1, d1) == new Angle(v2, d2));
    }

    [Theory]
    [InlineData(1, 4, 1, 4)]
    public void AngleCompT(int v1, int d1, int v2, int d2)
    {

        Assert.False(new Angle(v1, d1) != new Angle(v2, d2));
    }

    [Fact]
    public void AngleEquals()
    {
        Assert.False((new Angle(1)).Equals(null));
    }
}
