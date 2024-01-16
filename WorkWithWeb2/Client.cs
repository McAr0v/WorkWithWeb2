using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WorkWithWeb2
{
    internal class Client
    {

        public static void SendMsg(string name)
        {

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 16874);
            UdpClient udpClient = new UdpClient();
            Message msg = new Message(name, "Привет");

            string responseMsgJs = msg.ToJson();

            byte[] responseData = Encoding.UTF8.GetBytes(responseMsgJs);
            udpClient.Send(responseData, ep);
            byte[] answerData =  udpClient.Receive(ref ep);
            string answerMsgJs = Encoding.UTF8.GetString(answerData);
            Message answerMsg = Message.FromJson(answerMsgJs);

            Console.WriteLine(answerMsg.ToString());

        }
    }
}
