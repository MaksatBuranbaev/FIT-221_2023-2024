using System.Reflection;
using Hwdtech;
using Hwdtech.Ioc;
using Microsoft.CodeAnalysis;
using Moq;
namespace SpaceBattle.Lib.Tests;

public class CreateAdapterTest
{
    [Fact]
    public void CompileAdapterCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Adapter.Code",
            (object[] args) =>
            {
                return "SpaceBattle.Lib.Tests";
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Code.Compile",
            (object[] args) =>
            {
                return Assembly.Load((string)args[0]);
            }
        ).Execute();

        var map = new Dictionary<KeyValuePair<Type, Type>, Assembly>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Adapter.Map",
            (object[] args) =>
            {
                return map;
            }
        ).Execute();

        new CompileAdapterCommand(typeof(Type), typeof(Type)).Execute();

        Assert.Contains<KeyValuePair<Type, Type>>(new KeyValuePair<Type, Type>(typeof(Type), typeof(Type)), map.Keys);
    }

    [Fact]
    public void CreateAdapterStrategyTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var map = new Dictionary<KeyValuePair<Type, Type>, Assembly>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Adapter.Map",
            (object[] args) =>
            {
                return map;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Adapter.Find",
            (object[] args) =>
            {
                return " ";
            }
        ).Execute();

        var adapterCompileCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Adapter.Compile",
            (object[] args) =>
            {
                return adapterCompileCommand.Object;
            }
        ).Execute();

        var uobject = new Mock<IUObject>();

        new CreateAdapterStrategy().Run(uobject.Object, typeof(Type));

        adapterCompileCommand.Verify(p => p.Execute(), Times.Once);
    }

    [Fact]
    public void CompileCodeStrategy()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Assembly.Name.Create",
            (object[] args) => Guid.NewGuid().ToString()
        ).Execute();

        Assembly? assembly = null;
        var references = new List<MetadataReference> {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("SpaceBattle.Lib").Location)
        };
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Compile.References",
            (object[] args) => references
        ).Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Compile",
            (object[] args) => new CompileCodeStrategy().Run(args)
        ).Execute();

        var adapterCode = @"namespace SpaceBattle.Lib;
                            public class Test {
                            public Test() {}
                            }";

        assembly = IoC.Resolve<Assembly>("Compile", adapterCode);

        var f = Activator.CreateInstance(assembly.GetType("SpaceBattle.Lib.Test")!)!;

        Assert.Equal("SpaceBattle.Lib.Test", f.GetType().ToString());
    }
}
