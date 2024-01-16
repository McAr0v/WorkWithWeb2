using System;
namespace WorkWithWeb2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string? exit = null;

            while (exit == null || exit.ToLower() != "exit")
            {
                if (args.Length == 0)
                {
                    Server.AcceptMsg();
                    
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        new Thread(() => {

                            Client.SendMsg($"Alex {i}");

                        }).Start();
                    }
                }

                exit = Convert.ToString(Console.ReadLine());

            }
            Environment.Exit(0);

        }
    }
}