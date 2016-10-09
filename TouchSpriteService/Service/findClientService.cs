using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TouchSpriteService.Model;

namespace TouchSpriteService
{
    /// <summary>
    /// 思路：通过各个手机设备返回的IP以及端口，对其请求交换数据
    /// IP+端口+命令路径
    /// </summary>
    class findClientService
    {
        public List<ClientInfo> list = new List<ClientInfo>();
        public void findClient()
        {
            Console.WriteLine("Server:");
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 14099);
            byte[] buf = Encoding.Default.GetBytes("{\"ip\":\"192.168.0.50\", \"port\":11020}");

            Thread t = new Thread(new ThreadStart(RecvThread));
            t.IsBackground = true;
            t.Start();

            int i = 0;
            while (true && i < 10)
            {
                i++;
                client.Send(buf, buf.Length, endpoint);
                Thread.Sleep(1000);
            }

            Common.IOHelper.SerializeToXml(list, "clients.xml");
        }


        public void RecvThread()
        {
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 11020));
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                byte[] buf = client.Receive(ref endpoint);
                string resultJson = Encoding.Default.GetString(buf);

                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientInfo>(resultJson);
                if (list.Where(q=>q.deviceid==model.deviceid).Count()==0)
                {
                    list.Add(model);
                }

                Console.WriteLine(resultJson);
            }
        }
    }
}
