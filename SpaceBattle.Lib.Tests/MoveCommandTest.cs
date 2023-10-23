using System.Data;
using Moq;
using TechTalk.SpecFlow;
namespace SpaceBattle.Lib.Tests;

delegate void F();

[Binding, Scope(Feature = "Прямолинейное движение")]
public class MoveCommandTest
{
    private static Mock<IMovable> movable = new Mock<IMovable>();
    private F? a;
    [Given(@"космический корабль находится в точке пространства с координатами \((.*), (.*)\)")]
         public void ДопустимКосмическийКорабльНаходитсяВТочкеПространстваСКоординатами(int p0, int p1)
         {
            movable.SetupGet(m => m.Position).Returns(new Vectorn(new int[] { p0, p1 })).Verifiable();
         }

    [Given(@"изменить положение в пространстве космического корабля невозможно")]
         public void ДопустимИзменитьПоложениеВПространствеКосмическогоКорабляНевозможно()
         {
            movable.SetupGet(m => m.Position).Throws<Exception>().Verifiable();
         }

    [Given(@"космический корабль, положение в пространстве которого невозможно определить")]
         public void ДопустимКосмическийКорабльПоложениеВПространствеКоторогоНевозможноОпределить()
         {
            movable.SetupGet(m => m.Position).Throws<Exception>().Verifiable();
         }

    [Given(@"имеет мгновенную скорость \((.*), (.*)\)")]
         public void ДопустимИмеетМгновеннуюСкорость(int p0, int p1)
         {
            movable.SetupGet(m => m.Velocity).Returns(new Vectorn(new int[] { p0, p1 })).Verifiable();
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
            movable.VerifySet(m => m.Position = new Vectorn(new int[] { p0, p1 }), Times.Once);
         }
}

