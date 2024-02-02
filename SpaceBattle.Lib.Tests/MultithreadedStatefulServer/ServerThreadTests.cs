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

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Dictionary.Threads",
            (object[] args) =>
            {
                var threads = new Dictionary<int, object>() { };
                return threads;
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Create And Start Thread",
            (object[] args) =>
            {
                var st = new ServerThread(_q);
                var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
                var act = (Action)args[1];

                var startThreadCommand = new Mock<ICommand>();
                startThreadCommand.Setup(stc => stc.Execute()).Callback(new Action(() =>
                {
                    st.Start();
                    threads.Add((int)args[0], st);
                    act();
                }));
                return startThreadCommand;
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Send Command",
            (object[] args) =>
            {
                var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
                var st = (ServerThread)threads[(int)args[0]];

                var sendCommand = new Mock<ICommand>();
                sendCommand.Setup(sc => sc.Execute()).Callback(new Action(() =>
                {
                    st.Add((ICommand)args[1]);
                }));
                return sendCommand;
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Hard Stop The Thread",
            (object[] args) =>
            {
                var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
                var st = (ServerThread)threads[(int)args[0]];
                var act = (Action)args[1];

                var hardStopCommand = new Mock<ICommand>();
                hardStopCommand.Setup(hcs => hcs.Execute()).Callback(new Action(() =>
                {
                    act();
                    var stop = new Mock<ICommand>();
                    stop.Setup(s => s.Execute()).Callback(new Action(() => st.Stop()));
                    IoC.Resolve<ICommand>("Send Command", args[0], stop).Execute();
                }));
                return hardStopCommand;
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Soft Stop The Thread",
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
                                IoC.Resolve<ICommand>("Hard Stop The Thread", args[0], act).Execute();
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
                    IoC.Resolve<ICommand>("Send Command", args[0], stop).Execute();
                }));
                return softStopCommand;
            }
        ).Execute();
    }
    [Fact]
    public void CorrectServerThreadTest()
    {

    }
}
