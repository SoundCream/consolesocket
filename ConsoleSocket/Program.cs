// See https://aka.ms/new-console-template for more information
using ConsoleSocket;

Console.WriteLine("Hello, World!");


var msg = Console.ReadLine();
var sy = new SyncDataService();
if (msg == "1")
{
    sy.OpenSocket((x) => {
        Console.WriteLine(x);
    });

    for (int i = 0; i < 1000; i++)
    {
        Thread.Sleep(1000);
        Console.WriteLine(i);
    }
}
else
{
    while (true)
    {
        sy.SendMessage(msg);
        msg = Console.ReadLine();
    }
}



Console.ReadLine();