using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkWithWeb2
{
    internal class Server
    {
        static private CancellationTokenSource cts = new CancellationTokenSource();
        static private CancellationToken ct = cts.Token;
        public static async Task AcceptMsg()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            UdpClient udpClient = new UdpClient(16874);
            Console.WriteLine("Сервер ожидает сообщение");

            try
            {
                while (!ct.IsCancellationRequested) {

                    var data = udpClient.Receive(ref ep);

                    string data1 = Encoding.UTF8.GetString(data);

                    Message msg = Message.FromJson(data1);
                 
                    await Task.Run(async () =>
                    {
                        
                        Console.WriteLine(msg.ToString());
                        Message responseMsg = new Message("Server", "Message accept on serv");
                        string responseMsgJs = responseMsg.ToJson();
                        byte[] responseDate = Encoding.UTF8.GetBytes(responseMsgJs);
                        await udpClient.SendAsync(responseDate, ep);
                    });

                    if (msg.Text.ToLower() == "exit")
                    {
                        Console.WriteLine("Operation was canceled.");
                        ct.ThrowIfCancellationRequested();
                        break;

                    }

                }
            }
            catch(OperationCanceledException) {

                
                Environment.Exit(0);

            }

        }
    }
}
