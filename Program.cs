﻿using Hwdtech;

namespace SpaceBattle.Lib;

internal class Program
{
    private static void Main(string[] args)
    {
        var countThreads = int.Parse(args[0]);

        IoC.Resolve<ICommand>("StartServer", countThreads).Execute();

        Console.WriteLine("Для остановки сервера нажмите любую клавишу...");
        Console.ReadKey();

        IoC.Resolve<ICommand>("StopServer").Execute();
        Console.WriteLine("Сервер остановлен");
    }
}
