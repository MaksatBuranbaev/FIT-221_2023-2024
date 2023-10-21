using Moq;
using TechTalk.SpecFlow;
namespace SpaceBattle.Lib.Tests;

[Binding]
public class RotateCommandTest
{
   private Mock<SpaceBattle.Lib.IRotateble> rotateble = new Mock<IRotateble>();
   //private ICommand rotateCommand = new RotateCommand(rotateble.Object);
   private Exception actualException = new Exception();

   [Given(@"космический корабль имеет угол наклона (.*) град к оси OX")]
   public void ДанУголНаклона(int angle)
   {
      rotateble.SetupGet(m => m.Angle).Returns(new Angle(angle)).Verifiable();
   }

   [Given(@"космический корабль, угол наклона которого невозможно определить")]
   public void УголНаклонаНевозможноОпределить()
   {
      rotateble.SetupGet(m => m.Angle).Throws<Exception>().Verifiable();
   }

   [Given(@"имеет мгновенную угловую скорость (.*) град")]
   public void ДанаУгловаяСкорость(int rotateSpeed)
   {
      rotateble.SetupGet(m => m.RotationalSpeed).Returns(new Angle(rotateSpeed)).Verifiable();
   }

   [Given(@"мгновенную угловую скорость невозможно определить")]
   public void НевозможноОпределитьУгловуюСкорость()
   {
      rotateble.SetupGet(m => m.RotationalSpeed).Throws<Exception>().Verifiable();
   }

   [Given(@"невозможно изменить угол наклона к оси OX космического корабля")]
   public void НевозможноИзменитьУголНаклона()
   {
      
   }

   [When(@"происходит вращение вокруг собственной оси")]
   public void ПроисходитВращение()
   {
      ICommand rotateCommand = new RotateCommand(rotateble.Object);
      try{
         rotateCommand.Execute();
      }
      catch(Exception e)
      {
         actualException = e;
      }
   }

   [Then(@"угол наклона космического корабля к оси OX составляет (.*) град")]
   public void УголНаклонаСоставляет(int angle)
   {
      rotateble.VerifySet(m => m.Angle = new Angle(angle), Times.Once);
      //rotateble.VerifyAll();
   }

   [Then(@"возникает ошибка Exception")]
   public void ВозникаетОшибка()
   {
      Assert.ThrowsAsync<Exception>(() => throw actualException);
   }

}