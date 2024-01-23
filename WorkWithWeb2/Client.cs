using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WorkWithWeb2
{
    

    internal class Client
    {

        static private CancellationTokenSource cts2 = new CancellationTokenSource();
        static private CancellationToken ct2 = cts2.Token;

        public static class MessageFactory
        {
            public static Message CreateMessage(string sender, string receiver, string text)
            {
                return new Message(sender, text)
                {
                    ToName = receiver
                };
            }
        }

        public static async Task ClientSendler(string name)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 16874);
            UdpClient udpClient = new UdpClient();

            try
            {
                while (!ct2.IsCancellationRequested)
                {
                    Console.Write("Введите имя получателя: ");

                    string whoIsGetThisMessage = await Task.Run(() => Console.ReadLine());

                    if (String.IsNullOrEmpty(whoIsGetThisMessage))
                    {
                        Console.WriteLine("Вы не ввели имя пользователя");
                        continue;
                    }

                    Console.Write("Введите сообщение или Exit для завершения: ");

                    string text = await Task.Run(() => Console.ReadLine());

                    // Message msg = new Message(name, text ?? "Привет");
                    Message msg = MessageFactory.CreateMessage(name, whoIsGetThisMessage, text ?? "Привет");

                    msg.ToName = whoIsGetThisMessage;

                    string responseMsgJs = msg.ToJson();

                    byte[] responseData = Encoding.UTF8.GetBytes(responseMsgJs);

                    await udpClient.SendAsync(responseData, responseData.Length, ep);

                    byte[] answerData = udpClient.Receive(ref ep);
                    string answerMsgJs = Encoding.UTF8.GetString(answerData);
                    Message answerMsg = Message.FromJson(answerMsgJs);

                    Console.WriteLine(answerMsg.ToString());

                    if (text.ToLower() == "exit")
                    {
                        Console.WriteLine("Operation was canceled.");
                        ct2.ThrowIfCancellationRequested();
                        break;

                    }

                }
            }
            catch (OperationCanceledException)
            {
                Environment.Exit(0);
            }

            Environment.Exit(0);
        }

        public static async Task ClientListener()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 16874);
            UdpClient udpClient = new UdpClient();

            while (true)
            {

                byte[] answerData = udpClient.Receive(ref ep);
                string answerMsgJs = Encoding.UTF8.GetString(answerData);
                Message answerMsg = Message.FromJson(answerMsgJs);

                Console.WriteLine(answerMsg.ToString());

            }
        }

        public static async Task SendMsg(string name)
        {
           
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 16874);
            UdpClient udpClient = new UdpClient();

            try
            {
                while (!ct2.IsCancellationRequested)
                {
                    Console.Write("Введите имя получателя: ");

                    string whoIsGetThisMessage = Console.ReadLine();

                    if (String.IsNullOrEmpty(whoIsGetThisMessage))
                    {
                        Console.WriteLine("Вы не ввели имя пользователя");
                        continue;
                    }

                    Console.Write("Введите сообщение или Exit для завершения: ");

                    string text = Console.ReadLine();

                    Message msg = new Message(name, text ?? "Привет");

                    msg.ToName = whoIsGetThisMessage;

                    string responseMsgJs = msg.ToJson();

                    byte[] responseData = Encoding.UTF8.GetBytes(responseMsgJs);

                    await udpClient.SendAsync(responseData, responseData.Length, ep);

                    byte[] answerData = udpClient.Receive(ref ep);
                    string answerMsgJs = Encoding.UTF8.GetString(answerData);
                    Message answerMsg = Message.FromJson(answerMsgJs);

                    Console.WriteLine(answerMsg.ToString());

                    if (text.ToLower() == "exit")
                    {
                        Console.WriteLine("Operation was canceled.");
                        ct2.ThrowIfCancellationRequested();
                        break;

                    }

                }
            }
            catch (OperationCanceledException)
            {
                Environment.Exit(0);
            }

            Environment.Exit(0);

        }
    }
}
