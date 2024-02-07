using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;

public class ServerThreadTest
{
    private static readonly BlockingCollection<ICommand>? _q;

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
                if((int)args[0] != st.GetId())
                    {
                        throw new ArgumentException("Inappropriate thread");
                    }
                var act = (Action)args[1];

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
                if((int)args[0] != st.GetId())
                {
                    throw new ArgumentException("Inappropriate thread");
                }
                
                var act = (Action)args[1];
                var softStopCommand = new Mock<ICommand>();
                var _q = st.GetQ();
                softStopCommand.Setup(stc => stc.Execute()).Callback(new Action(() =>
                {
                    act();
                    var stop = new Mock<ICommand>();
                    stop.Setup(s => s.Execute()).Callback(new Action(() =>
                    {
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
                        st.UpdateBehaviour(strategy);
                    }));
                    IoC.Resolve<ICommand>("SendCommand", args[0], stop.Object).Execute();
                }));
                return softStopCommand.Object;
            }
        ).Execute();
    }
    [Fact]
    public void StartServerThreadTest()
    {
        var q = new BlockingCollection<ICommand>();
        var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
        var act = () => Console.WriteLine("Start!");
        IoC.Resolve<ICommand>("CreateAndStartThread", 1, act, q).Execute();
        var st = (ServerThread)threads[1];
        Thread.Sleep(2);

        Assert.True(threads.Count != 0);
        Assert.True(st.GetThread().IsAlive);
    }
    [Fact]
    public void HardServerThreadTest()
    {
        var q = new BlockingCollection<ICommand>();
        var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
        var act1 = () => Console.WriteLine("Start!");
        IoC.Resolve<ICommand>("CreateAndStartThread", 1, act1, q).Execute();
        
        var st = (ServerThread)threads[1];
        var act2 = () => Console.WriteLine("Stop!");
        IoC.Resolve<ICommand>("HardStopTheThread", 1, act2).Execute();
        Thread.Sleep(2);

        Assert.True(threads.Count == 0);
        Assert.False(st.GetThread().IsAlive);
    }
    [Fact]
    public void SoftServerThreadTest()
    {
        var q = new BlockingCollection<ICommand>();
        var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
        var act1 = () => Console.WriteLine("Start!");
        IoC.Resolve<ICommand>("CreateAndStartThread", 3, act1, q).Execute();

        var st = (ServerThread)threads[3];
        var act2 = () => Console.WriteLine("Stop!");

        var stop = new Mock<ICommand>();
        stop.Setup(s => s.Execute()).Callback(new Action(() => 
            {
                Console.WriteLine("1234");
            }));
        IoC.Resolve<ICommand>("SendCommand", 3, stop.Object).Execute();

        IoC.Resolve<ICommand>("SoftStopTheThread", 3, act2).Execute();

        var cmd1 = new Mock<ICommand>();
        cmd1.Setup(s => s.Execute()).Callback(new Action(() => 
            {
                Console.WriteLine("1234");
            }));
        var cmd2 = new Mock<ICommand>();
        IoC.Resolve<ICommand>("SendCommand", 3, cmd1.Object).Execute();
        IoC.Resolve<ICommand>("SendCommand", 3, cmd2.Object).Execute();
        Thread.Sleep(2);

        stop.Verify();
        cmd1.Verify();
        cmd2.Verify();
        Assert.True(threads.Count == 0);
        Assert.False(st.GetThread().IsAlive);
    }
}
