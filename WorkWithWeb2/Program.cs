using System;
namespace WorkWithWeb2
{
    internal class Program
    {

        static async Task Main(string[] args)
        {

            if (args.Length == 0)
            {
                await Server.AcceptMsg();

            }
            else
            {
                Task sendlerTask = Task.Run(async () => await Client.ClientSendler(args[0]));

                // Дождаться завершения ClientSendler
                //await sendlerTask;

                // Запустить ClientListener в отдельном потоке
                Task listenerTask = Task.Run(async () => await Client.ClientListener());

                await Task.WhenAll(sendlerTask, listenerTask);

                // await Client.ClientSendler(args[0]);

                // await Client.ClientListener();


                //await Client.SendMsg(args[0]);

            }

        }
    }
}