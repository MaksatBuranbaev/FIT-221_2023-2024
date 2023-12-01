using Moq;
using TechTalk.SpecFlow;
namespace SpaceBattle.Lib.Tests;

internal delegate void F();

[Binding, Scope(Feature = "Прямолинейное движение")]
public class MoveCommandTest
{
    private readonly Mock<IMovable> movable = new Mock<IMovable>();
    private F? a;
    [Given(@"космический корабль находится в точке пространства с координатами \((.*), (.*)\)")]
    public void ДопустимКосмическийКорабльНаходитсяВТочкеПространстваСКоординатами(int p0, int p1)
    {
        movable.SetupGet(m => m.Position).Returns(new Vector(new int[] { p0, p1 })).Verifiable();
    }

    [Given(@"изменить положение в пространстве космического корабля невозможно")]
    public void ДопустимИзменитьПоложениеВПространствеКосмическогоКорабляНевозможно()
    {
        movable.Setup(m => m.Position).Throws<Exception>().Verifiable();
    }

    [Given(@"космический корабль, положение в пространстве которого невозможно определить")]
    public void ДопустимКосмическийКорабльПоложениеВПространствеКоторогоНевозможноОпределить()
    {
        movable.SetupGet(m => m.Position).Throws<Exception>().Verifiable();
    }

    [Given(@"имеет мгновенную скорость \((.*), (.*)\)")]
    public void ДопустимИмеетМгновеннуюСкорость(int p0, int p1)
    {
        movable.SetupGet(m => m.Velocity).Returns(new Vector(new int[] { p0, p1 })).Verifiable();
    }

    [Given(@"скорость корабля определить невозможно")]
    public void ДопустимСкоростьКорабляОпределитьНевозможно()
    {
        movable.SetupGet(m => m.Velocity).Throws<Exception>().Verifiable();
    }

    [When(@"происходит прямолинейное равномерное движение без деформации")]
    public void КогдаПроисходитПрямолинейноеРавномерноеДвижениеБезДеформации()
    {
        a = () => (new MoveCommand(movable.Object)).Execute();
    }

    [Then(@"возникает ошибка Exception")]
    public void ТоВозникаетОшибкаException()
    {
        Assert.Throws<Exception>(() => a());
    }

    [Then(@"космический корабль перемещается в точку пространства с координатами \((.*), (.*)\)")]
    public void ТоКосмическийКорабльПеремещаетсяВТочкуПространстваСКоординатами(int p0, int p1)
    {
        a();
        movable.VerifySet(m => m.Position = new Vector(new int[] { p0, p1 }), Times.Once);
    }
}

[Binding, Scope(Feature = "Вектор")]
public class VectorTest
{
    private Vector vec;
    private int hash;
    private F? a;
    [Given(@"вектор принимает на вход массив \((.*), (.*)\)")]
    public void ДопустимВекторПринимаетНаВходМассив(int p0, int p1)
    {
        vec = new Vector(new int[] { p0, p1 });
        hash = vec.GetHashCode();
    }

    [Given(@"массив на вход невозможно определить")]
    public void ДопустимМассивНаВходНевозможноОпределить()
    {
        vec = new Vector(new int[] { });
        hash = vec.GetHashCode();
    }

    [When(@"происходит сложение с другим вектором \((.*), (.*)\)")]
    public void КогдаПроисходитСложениеСДругимВектором(int p0, int p1)
    {
        a = () => vec += new Vector(new int[] { p0, p1 });
    }

    [When(@"происходит вычитание с другим вектором \((.*), (.*)\)")]
    public void КогдаПроисходитВычитаниеСДругимВектором(int p0, int p1)
    {
        a = () => vec -= new Vector(new int[] { p0, p1 });
    }

    [When(@"происходит сложение с неизвестным вектором")]
    public void КогдаПроисходитСложениеСНеизвестнымВектором()
    {
        a = () => vec -= new Vector(new int[] { });
    }

    [Then(@"вектор равен массиву \((.*), (.*)\)")]
    public void ТоВекторРавенМассиву(int p0, int p1)
    {
        a();
        Assert.True(vec == new Vector(new int[] { p0, p1 }));
    }

    [Then(@"вектор не равен массиву \((.*), (.*)\)")]
    public void ТоВекторНеРавенМассиву(int p0, int p1)
    {
        a();
        Assert.True(vec != new Vector(new int[] { p0, p1 }));
    }

    [Then(@"происходит сравнение с null")]
    public void ТоПроисходитСравнениеСNull()
    {
        a();
        Assert.False(vec.Equals(null));
    }

    [Then(@"возникает ошибка Exception")]
    public void ТоВозникаетОшибкаException()
    {
        Assert.Throws<Exception>(() => a());
    }

    [Then(@"HashCode не изменится")]
    public void ТоHashCodeНеИзменится()
    {
        Assert.True(vec.GetHashCode() == hash);
    }
}
