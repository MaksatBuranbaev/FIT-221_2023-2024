﻿using System.Collections.Concurrent;
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
                var st = new ServerThread((BlockingCollection<ICommand>)args[2]);
                var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
                var act = (Action)args[1] ?? (() => Console.WriteLine("Stop!"));

                var startThreadCommand = new Mock<ICommand>();
                startThreadCommand.Setup(stc => stc.Execute()).Callback(new Action(() =>
                {
                    st.Start();
                    act();
                    threads.Add((int)args[0], st);
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
                var act = (Action)args[1] ?? (() => Console.WriteLine("Stop!"));
                var hs = new HardStopCommand(st, act);

                var hardStopCommand = new Mock<ICommand>();
                hardStopCommand.Setup(hcs => hcs.Execute()).Callback(new Action(() =>
                {
                    st.Add(hs);
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
                }));
                return softStopCommand.Object;
            }
        ).Execute();
    }
    [Fact]
    public void StartServerThreadTest()
    {
        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<ICommand>();
        var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
        Action act = () => mre.Set();
        IoC.Resolve<ICommand>("CreateAndStartThread", 0, act, q).Execute();
        var st = (ServerThread)threads[0];
        mre.WaitOne();

        Assert.True(threads.Count != 0);
        Assert.True(st.GetThread().IsAlive);

        threads.Remove(0);
    }
    [Fact]
    public void HardStopServerThreadTest()
    {
        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<ICommand>();
        var act = () => Console.WriteLine("Start!");
        Action act2 = () => mre.Set();

        IoC.Resolve<ICommand>("CreateAndStartThread", 1, act, q).Execute();
        var hs = IoC.Resolve<ICommand>("HardStopTheThread", 1, act2);
        var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
        var st = (ServerThread)threads[1];

        var cmd = new Mock<ICommand>();
        cmd.Setup(c => c.Execute()).Callback(new Action(() =>
        {
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )).Execute();
        }));

        var eh = new Mock<ICommand>();
        eh.Setup(e => e.Execute()).Callback(new Action(() =>
        {
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

        var cmd0 = new Mock<ICommand>();
        cmd0.Setup(c => c.Execute()).Throws<Exception>().Verifiable();
        var cmd1 = new Mock<ICommand>();
        var cmd2 = new Mock<ICommand>();

        q.Add(cmd.Object);
        q.Add(eh.Object);
        q.Add(cmd0.Object);
        q.Add(cmd1.Object);
        q.Add(hs);
        q.Add(cmd2.Object);
        mre.WaitOne();

        cmd0.Verify(c => c.Execute(), Times.Once());
        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
        Assert.False(st.GetThread().IsAlive);

        threads.Remove(1);
    }
    [Fact]
    public void SoftStopServerThreadTest()
    {
        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<ICommand>();
        var act = () => Console.WriteLine("Start!");
        IoC.Resolve<ICommand>("CreateAndStartThread", 2, act, q).Execute();
        var threads = IoC.Resolve<Dictionary<int, object>>("Dictionary.Threads");
        var st = (ServerThread)threads[2];
        Action act2 = () =>
        {
            mre.Set();
        };
        var ss = IoC.Resolve<ICommand>("SoftStopTheThread", 2, act2);

        var cmd0 = new Mock<ICommand>();
        var cmd1 = new Mock<ICommand>();
        var cmd2 = new Mock<ICommand>();

        q.Add(cmd0.Object);
        q.Add(ss);
        q.Add(cmd1.Object);
        q.Add(cmd2.Object);

        mre.WaitOne();

        cmd0.Verify(c => c.Execute(), Times.Once());
        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());

        Thread.Sleep(10);
        Assert.False(st.GetThread().IsAlive);

        threads.Remove(2);
    }
}
