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
        
        private static void Register ()
        {

        }



        static private CancellationTokenSource cts = new CancellationTokenSource();
        static private CancellationToken ct = cts.Token;
        public static async Task AcceptMsg()
        {
            Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();

            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            UdpClient udpClient = new UdpClient(16874);
            Console.WriteLine("Сервер ожидает сообщение");

            try
            {
                while (!ct.IsCancellationRequested) {

                    var data = udpClient.Receive(ref ep);

                    string data1 = Encoding.UTF8.GetString(data);

                    Message msg = Message.FromJson(data1);

                    if (msg != null) {

                        Message newMsg = new Message();

                        await Task.Run(async () =>
                        {
                            
                            if (msg != null)
                            {

                                if (msg.ToName.Equals("Server"))
                                {
                                    if (msg.Text.ToLower().Equals("register"))
                                    {
                                        if (clients.TryAdd(msg.FromName, ep))
                                        {
                                            newMsg = new Message("Server", $"Клиент {msg.FromName} добавлен");
                                        }
                                    }
                                    else if (msg.Text.ToLower().Equals("delete"))
                                    {
                                        clients.Remove(msg.FromName);
                                        newMsg = new Message("Server", $"Клиент {msg.FromName} удален");
                                    }
                                    else if (msg.Text.ToLower().Equals("list"))
                                    {
                                        StringBuilder stringBuilder = new StringBuilder();
                                        foreach (var client in clients)
                                        {
                                            stringBuilder.Append(client.Key + "\n" );
                                        }

                                        newMsg = new Message("Server", $"Cписок зарегистрированных клиентов: \n {stringBuilder.ToString()}");

                                    }

                                }
                                else if (msg.ToName.ToLower().Equals("all"))
                                {
                                    foreach (var client in clients)
                                    {
                                        msg.ToName = client.Key;
                                        string tempJs = msg.ToJson();
                                        byte[] tempResponseDate = Encoding.UTF8.GetBytes(tempJs);
                                        await udpClient.SendAsync(tempResponseDate, client.Value);

                                    }

                                    newMsg = new Message("Server", "Сообщение отправлено всем клиентам");

                                }
                                
                                else if (clients.TryGetValue(key: msg.ToName, out IPEndPoint? value))
                                {
                                    string tempJs = msg.ToJson();
                                    byte[] tempResponseDate = Encoding.UTF8.GetBytes(tempJs);
                                    await udpClient.SendAsync(tempResponseDate, value);

                                    newMsg = new Message("Server", $"Пользователю {msg.ToName} отправлено сообщение");

                                }
                                else 
                                {
                                    newMsg = new Message("Server", $"Пользователь {msg.ToName} не существует");

                                }

                            }

                            Console.WriteLine(msg.ToString());
                            // Message responseMsg = new Message("Server", "Message accept on serv");
                            string responseMsgJs = newMsg.ToJson();
                            byte[] responseDate = Encoding.UTF8.GetBytes(responseMsgJs);
                            await udpClient.SendAsync(responseDate, ep);
                        });

                    }

                    

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
