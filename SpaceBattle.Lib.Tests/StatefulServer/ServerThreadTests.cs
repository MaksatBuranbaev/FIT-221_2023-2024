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

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Dictionary.Threads",
            (object[] args) =>
            {
                var threads = new Dictionary<int, object>() { };
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
                var act = (Action)args[1];

                var hardStopCommand = new Mock<ICommand>();
                hardStopCommand.Setup(hcs => hcs.Execute()).Callback(new Action(() =>
                {
                    if((int)args[0] != st.GetId())
                    {
                        throw new ArgumentException("Inappropriate thread");
                    }
                    act();
                    var stop = new Mock<ICommand>();
                    stop.Setup(s => s.Execute()).Callback(new Action(() => st.Stop()));
                    IoC.Resolve<ICommand>("SendCommand", args[0], stop).Execute();
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
                var act = (Action)args[1];
                var softStopCommand = new Mock<ICommand>();
                softStopCommand.Setup(stc => stc.Execute()).Callback(new Action(() =>
                {
                    act();
                    var stop = new Mock<ICommand>();
                    stop.Setup(s => s.Execute()).Callback(new Action(() =>
                    {
                        st.UpdateBehaviour(() =>
                        {
                            if (_q.Count == 0)
                            {
                                IoC.Resolve<ICommand>("HardStopTheThread", args[0], act).Execute();
                            }

                            var cmd = _q.Take();
                            try
                            {
                                cmd.Execute();
                            }
                            catch (Exception e)
                            {
                                IoC.Resolve<ICommand>("Exception.Handle", cmd, e).Execute();
                            }
                        });
                    }));
                    IoC.Resolve<ICommand>("SendCommand", args[0], stop).Execute();
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
        Assert.NotNull(threads);
    }
}
