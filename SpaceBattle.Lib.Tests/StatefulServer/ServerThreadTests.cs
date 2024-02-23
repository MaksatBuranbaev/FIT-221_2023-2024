using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;

public class ServerThreadTest
{
    static ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        var threads = new Dictionary<int, object>() { };
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Dictionary.Threads",
            (object[] args) =>
            {
                return threads;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "CreateAndStartThread",
            (object[] args) =>
            {
                var st = new ServerThread((BlockingCollection<ICommand>)args[2], (int)args[0]);
                var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
                var act = (Action)args[1];

                var startThreadCommand = new Mock<ICommand>();
                startThreadCommand.Setup(stc => stc.Execute()).Callback(new Action(() =>
                {
                    st.Start();
                    threads.Add((int)args[0], st);
                    act();
                }));
                return startThreadCommand.Object;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "SendCommand",
            (object[] args) =>
            {
                var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
                var st = (ServerThread)threads[(int)args[0]];

                var sendCommand = new Mock<ICommand>();
                sendCommand.Setup(sc => sc.Execute()).Callback(new Action(() =>
                {
                    st.Add((ICommand)args[1]);
                }));
                return sendCommand.Object;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "HardStopTheThread",
            (object[] args) =>
            {
                var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
                var st = (ServerThread)threads[(int)args[0]];
                if ((int)args[0] != st.GetId())
                {
                    throw new ArgumentException("Inappropriate thread");
                }

                var act = (Action)args[1] ?? (() => Console.WriteLine("Stop!"));

                var hardStopCommand = new Mock<ICommand>();
                hardStopCommand.Setup(hcs => hcs.Execute()).Callback(new Action(() =>
                {
                    act();
                    var stop = new Mock<ICommand>();
                    stop.Setup(s => s.Execute()).Callback(new Action(() =>
                    {
                        st.Stop();
                        threads.Remove((int)args[0]);
                    }));
                    IoC.Resolve<ICommand>("SendCommand", args[0], stop.Object).Execute();
                }));
                return hardStopCommand.Object;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "SoftStopTheThread",
            (object[] args) =>
            {
                var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
                var st = (ServerThread)threads[(int)args[0]];
                if ((int)args[0] != st.GetId())
                {
                    throw new ArgumentException("Inappropriate thread");
                }

                var act = (Action)args[1] ?? (() => Console.WriteLine("Stop!"));
                var softStopCommand = new Mock<ICommand>();
                var _q = st.GetQ();
                softStopCommand.Setup(stc => stc.Execute()).Callback(new Action(() =>
                {
                    act();
                    var stop = new Mock<ICommand>();
                    var strategy = () =>
                        {
                            if (_q.Count == 0)
                            {
                                threads.Remove((int)args[0]);
                                st.Stop();
                            }
                            else
                            {
                                var cmd = _q.Take();
                                try
                                {
                                    cmd.Execute();
                                }
                                catch (Exception e)
                                {
                                    IoC.Resolve<ICommand>("Exception.Handle", cmd, e).Execute();
                                }
                            }
                        };
                    stop.Setup(s => s.Execute()).Callback(new Action(() =>
                    {
                        st.UpdateBehaviour(strategy);
                    }));
                    IoC.Resolve<ICommand>("SendCommand", args[0], stop.Object).Execute();
                }));
                return softStopCommand.Object;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ExceptionHandler.Find",
            (object[] args) => 
            {
                var handler = new Mock<IExceptionHandler>();
                return handler.Object;
            }
        ).Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Exception.Handle",
            (object[] args) =>
            {
                var handler = IoC.Resolve<IExceptionHandler>("ExceptionHandler.Find", (ICommand)args[0], (Exception)args[1]);
                var cmd = new Mock<ICommand>();
                cmd.Setup(c => c.Execute()).Callback(new Action(() =>
                    {
                        handler.Handle();
                    }));
                return cmd.Object;
            }
        ).Execute();
    }
    [Fact]
    public void StartServerThreadTest()
    {
        var q = new BlockingCollection<ICommand>();
        var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
        var act = () => Console.WriteLine("Start!");
        IoC.Resolve<ICommand>("CreateAndStartThread", 0, act, q).Execute();
        var st = (ServerThread)threads[0];
        Thread.Sleep(10);

        Assert.True(threads.Count != 0);
        Assert.True(st.GetThread().IsAlive);
        IoC.Resolve<ICommand>("HardStopTheThread", 0, act).Execute();

        Thread.Sleep(500);
    }
    [Fact]
    public void HardServerThreadTest()
    {
        var q = new BlockingCollection<ICommand>();
        var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
        var act = () => Console.WriteLine("Start!");
        IoC.Resolve<ICommand>("CreateAndStartThread", 0, act, q).Execute();
        var st = (ServerThread)threads[0];

        var act2 = () => Console.WriteLine("Stop!");

        var cmd0 = new Mock<ICommand>();
        try{
            cmd0.Setup(s => s.Execute()).Callback(new Action(() =>
            {
               throw new Exception();
            }));
        }
        catch{}

        IoC.Resolve<ICommand>("SendCommand", 0, cmd0.Object).Execute();

        IoC.Resolve<ICommand>("HardStopTheThread", 0, act2).Execute();
        Thread.Sleep(100);

        Assert.True(threads.Count == 0);
        Assert.False(st.GetThread().IsAlive);
        Thread.Sleep(500);
    }
    [Fact]
    public void SoftServerThreadTest()
    {
        var q = new BlockingCollection<ICommand>();
        var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
        var act1 = () => Console.WriteLine("Start!");
        IoC.Resolve<ICommand>("CreateAndStartThread", 2, act1, q).Execute();

        var st = (ServerThread)threads[2];
        var act2 = () => Console.WriteLine("Stop!");

        var cmd0 = new Mock<ICommand>();
        cmd0.Setup(s => s.Execute()).Callback(new Action(() =>
            {
                Console.WriteLine("1234");
            }));
        IoC.Resolve<ICommand>("SendCommand", 2, cmd0.Object).Execute();

        IoC.Resolve<ICommand>("SoftStopTheThread", 2, act2).Execute();

        var cmd1 = new Mock<ICommand>();
        cmd1.Setup(s => s.Execute()).Callback(new Action(() =>
            {
                Console.WriteLine("1234");
            }));
        var cmd2 = new Mock<ICommand>();
        IoC.Resolve<ICommand>("SendCommand", 2, cmd1.Object).Execute();
        IoC.Resolve<ICommand>("SendCommand", 2, cmd2.Object).Execute();
        Thread.Sleep(50);

        cmd0.Verify();
        cmd1.Verify();
        cmd2.Verify();
        Assert.True(threads.Count == 0);
        Assert.False(st.GetThread().IsAlive);
        Thread.Sleep(500);
    }
}
