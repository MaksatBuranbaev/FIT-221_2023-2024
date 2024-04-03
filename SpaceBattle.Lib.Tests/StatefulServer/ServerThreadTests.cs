using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace SpaceBattle.Lib.Tests;

public class ServerThreadTest
{
    static ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        var threads = new Dictionary<Guid, object>() { };
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
                var st = new ServerThread((BlockingCollection<ICommand>)args[2]);
                var threads = IoC.Resolve<Dictionary<Guid, object>>("Dictionary.Threads");
                var act = (Action)args[1] ?? (() => Console.WriteLine("Stop!"));
                var sc = new CreateAndStartThread(st, act);

                var startThreadCommand = new Mock<ICommand>();
                startThreadCommand.Setup(stc => stc.Execute()).Callback(new Action(() =>
                {
                    sc.Execute();
                    threads.Add((Guid)args[0], st);
                }));
                return startThreadCommand.Object;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "SendCommand",
            (object[] args) =>
            {
                var threads = IoC.Resolve<Dictionary<Guid, object>>("Dictionary.Threads");
                var st = (ServerThread)threads[(Guid)args[0]];

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
                var threads = IoC.Resolve<Dictionary<Guid, object>>("Dictionary.Threads");
                var st = (ServerThread)threads[(Guid)args[0]];
                var act = (Action)args[1] ?? (() => Console.WriteLine("Stop!"));
                var hs = new HardStopCommand(st, act);

                var hardStopCommand = new Mock<ICommand>();
                hardStopCommand.Setup(hcs => hcs.Execute()).Callback(new Action(() =>
                {
                    st.Add(hs);
                    threads.Remove((Guid)args[0]);
                }));
                return hardStopCommand.Object;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "SoftStopTheThread",
            (object[] args) =>
            {
                var threads = IoC.Resolve<Dictionary<Guid, object>>("Dictionary.Threads");
                var st = (ServerThread)threads[(Guid)args[0]];
                var act = (Action)args[1] ?? (() => Console.WriteLine("Stop!"));
                var _q = st.GetQ();
                Action strategy = () =>
                {
                    if (_q.Count == 0)
                    {
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
                var ss = new SoftStopCommand(st, act, strategy);
                var softStopCommand = new Mock<ICommand>();
                softStopCommand.Setup(stc => stc.Execute()).Callback(new Action(() =>
                {
                    st.Add(ss);
                    threads.Remove((Guid)args[0]);
                }));
                return softStopCommand.Object;
            }
        ).Execute();
    }

    [Fact]
    public void HardStopServerThreadTest()
    {
        var hs_mre = new ManualResetEvent(false);

        var q = new BlockingCollection<ICommand>();
        var act = () => Console.WriteLine("Start!");
        Action act2 = () =>
        {
            hs_mre.Set();
        };

        var guid = Guid.NewGuid();
        IoC.Resolve<ICommand>("CreateAndStartThread", guid, act, q).Execute();
        var hs = IoC.Resolve<ICommand>("HardStopTheThread", guid, act2);
        var threads = IoC.Resolve<Dictionary<Guid, object>>("Dictionary.Threads");
        var st = (ServerThread)threads[guid];

        var reg = new Mock<ICommand>();
        reg.Setup(e => e.Execute()).Callback(new Action(() =>
        {
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ExceptionHandler.Find",
            (object[] args) =>
            {
                var handler = new Mock<IExceptionHandler>();
                return handler.Object;
            }).Execute();

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
                }).Execute();
        }));

        var exc = new Mock<ICommand>();
        exc.Setup(c => c.Execute()).Throws<Exception>().Verifiable();
        var cmd1 = new Mock<ICommand>();
        var cmd2 = new Mock<ICommand>();
        var cmd3 = new Mock<ICommand>();
        var cmd4 = new Mock<ICommand>();

        q.Add(reg.Object);
        q.Add(exc.Object);
        q.Add(cmd1.Object);
        q.Add(cmd2.Object);
        q.Add(hs);
        hs_mre.WaitOne();
        q.Add(cmd3.Object);
        q.Add(cmd4.Object);

        reg.Verify(c => c.Execute(), Times.Once());
        exc.Verify(c => c.Execute(), Times.Once());
        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
        cmd3.Verify(c => c.Execute(), Times.Never);
        cmd4.Verify(c => c.Execute(), Times.Never);
        using (st)
        {
            Assert.False(st.GetThread().IsAlive);
        }
    }

    [Fact]
    public void SoftStopServerThreadTest()
    {
        var commands_mre = new ManualResetEvent(false);
        var ss_mre = new ManualResetEvent(false);

        var q = new BlockingCollection<ICommand>();
        var act = () => Console.WriteLine("Start!");
        var guid = Guid.NewGuid();
        IoC.Resolve<ICommand>("CreateAndStartThread", guid, act, q).Execute();
        var threads = IoC.Resolve<Dictionary<Guid, object>>("Dictionary.Threads");
        var st = (ServerThread)threads[guid];
        Action act2 = () =>
        {
            commands_mre.WaitOne();
            ss_mre.Set();
        };
        var ss = IoC.Resolve<ICommand>("SoftStopTheThread", guid, act2);

        var cmd0 = new Mock<ICommand>();
        var cmd1 = new Mock<ICommand>();
        var cmd2 = new Mock<ICommand>();
        cmd2.Setup(c => c.Execute()).Callback(new Action(() =>
        {
            commands_mre.Set();
        }));

        q.Add(cmd0.Object);
        q.Add(ss);
        q.Add(cmd1.Object);
        q.Add(cmd2.Object);

        ss_mre.WaitOne();

        cmd0.Verify(c => c.Execute(), Times.Once());
        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());

        using (st)
        {
            Assert.False(st.GetThread().IsAlive);
        }
    }

    [Fact]
    public void ServerThreadEquals()
    {
        var mre = new ManualResetEvent(false);
        var q1 = new BlockingCollection<ICommand>();
        var q2 = new BlockingCollection<ICommand>();
        var act = () => Console.WriteLine("Start!");
        Action act2 = () => mre.Set();
        var g1 = Guid.NewGuid();
        var g2 = Guid.NewGuid();
        IoC.Resolve<ICommand>("CreateAndStartThread", g1, act, q1).Execute();
        IoC.Resolve<ICommand>("CreateAndStartThread", g2, act, q2).Execute();
        var threads = IoC.Resolve<Dictionary<Guid, object>>("Dictionary.Threads");
        var hs = IoC.Resolve<ICommand>("HardStopTheThread", g1, act2);
        var ss = IoC.Resolve<ICommand>("SoftStopTheThread", g2, act2);
        var st1 = (ServerThread)threads[g1];
        var st2 = (ServerThread)threads[g2];
        st1.GetHashCode();

        Assert.False(st1.Equals(st2.GetThread()));
        Assert.False(st1.Equals(null));
        Assert.True(st1.Equals(st1.GetThread()));

        var scope = new Mock<ICommand>();
        scope.Setup(c => c.Execute()).Callback(new Action(() =>
        {
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )).Execute();
        }));
        var eh = new Mock<ICommand>();
        var flag = new Mock<ICommand>();
        eh.Setup(e => e.Execute()).Callback(new Action(() =>
        {
            IoC.Resolve<Hwdtech.ICommand>(
                "IoC.Register",
                "Exception.Handle",
                (object[] args) =>
                {
                    var cmd = new Mock<ICommand>();
                    cmd.Setup(c => c.Execute()).Callback(new Action(() =>
                        {
                            flag.Object.Execute();
                        }));
                    return cmd.Object;
                }).Execute();
        }));
        q1.Add(scope.Object);
        q1.Add(eh.Object);
        q1.Add(ss);

        q2.Add(scope.Object);
        q2.Add(eh.Object);
        q2.Add(hs);

        /* mre.WaitOne(); */
        using (st1)
        {
            /* flag.Verify(f => f.Execute(), Times.Exactly(2)); */
            using (st2)
            {
            }
        }
    }
}
