using Moq;
using TechTalk.SpecFlow;
namespace SpaceBattle.Lib.Tests;

internal delegate void LFunc();

[Binding]
public class RotateCommandTest
{
    private readonly Mock<SpaceBattle.Lib.IRotateble> _rotatable = new Mock<IRotateble>();
    private LFunc? _a;

    [Given(@"космический корабль имеет угол наклона (.*) град к оси OX")]
    public void ДанУголНаклона(int angle)
    {
        _rotatable.SetupGet(m => m.Position).Returns(new Angle((angle / 360 * 8) % 8)).Verifiable();
    }

    [Given(@"космический корабль, угол наклона которого невозможно определить")]
    public void УголНаклонаНевозможноОпределить()
    {
        _rotatable.SetupGet(m => m.Position).Throws<Exception>().Verifiable();
    }

    [Given(@"имеет мгновенную угловую скорость (.*) град")]
    public void ДанаУгловаяСкорость(int rotateSpeed)
    {
        _rotatable.SetupGet(m => m.RotationalSpeed).Returns(new Angle((rotateSpeed / 360 * 8) % 8)).Verifiable();
    }

    [Given(@"мгновенную угловую скорость невозможно определить")]
    public void НевозможноОпределитьУгловуюСкорость()
    {
        _rotatable.SetupGet(m => m.RotationalSpeed).Throws<Exception>().Verifiable();
    }

    [Given(@"невозможно изменить угол наклона к оси OX космического корабля")]
    public void НевозможноИзменитьУголНаклона()
    {
        _rotatable.SetupGet(m => m.Position).Throws<Exception>().Verifiable();
        _rotatable.SetupGet(m => m.RotationalSpeed).Throws<Exception>().Verifiable();
    }

    [When(@"происходит вращение вокруг собственной оси")]
    public void ПроисходитВращение()
    {
        _a = () => (new RotateCommand(_rotatable.Object)).Execute();
    }

    [Then(@"угол наклона космического корабля к оси OX составляет (.*) град")]
    public void УголНаклонаСоставляет(int angle)
    {
        _a();
        _rotatable.VerifySet(m => m.Position = new Angle((angle / 360 * 8) % 8), Times.Once);
    }

    [Then(@"возникает ошибка Exception")]
    public void ВозникаетОшибка()
    {
        Assert.Throws<Exception>(() => _a());
    }
}
