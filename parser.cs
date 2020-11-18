using System;
using SharpPcap;
using PacketDotNet;

namespace list
{
    class Program
    {
        static void Main()
        {
            CaptureDeviceList deviceList = CaptureDeviceList.Instance;
            foreach (ICaptureDevice dev in deviceList)
            {
                Console.WriteLine("{0}\n", dev.ToString());
            }
            ICaptureDevice device = deviceList[3];
            device.Open(DeviceMode.Promiscuous, 1000);

            Console.WriteLine();
            Console.WriteLine("-- Listening on {0}...", device.Description);

            Packet packet = null;
                 
            while(true)
            {
                RawCapture raw = device.GetNextPacket();
                while (raw == null) raw = device.GetNextPacket();
                packet = Packet.ParsePacket(raw.LinkLayerType, raw.Data);
                var tcpPacket = TcpPacket.GetEncapsulated(packet);
                var ipPacket = IpPacket.GetEncapsulated(packet);
                if (tcpPacket != null && ipPacket != null)
                {
                    DateTime time = raw.Timeval.Date;
                    int len = raw.Data.Length;
                    Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
                                       time.Hour, time.Minute, time.Second,
                                       time.Millisecond, len);
                    //Console.WriteLine(e.Packet.ToString());
                    // IP адрес отправителя
                    var srcIp = ipPacket.SourceAddress.ToString();
                    //Console.WriteLine("srcIp="+ srcIp);
                    // IP адрес получателя
                    var dstIp = ipPacket.DestinationAddress.ToString();
                    //Console.WriteLine("dstIp=" + dstIp);
                    // порт отправителя
                    var srcPort = tcpPacket.SourcePort.ToString();
                    //Console.WriteLine("srcPort=" + srcPort);
                    // порт получателя
                    var dstPort = tcpPacket.DestinationPort.ToString();
                    //Console.WriteLine("dstPost=" + dstPort);
                    // данные пакета
                    var data = BitConverter.ToString(raw.Data);
                    //Console.WriteLine("data=" + data);
                    string sendNTP = srcIp.ToString() + " " + dstIp.ToString() + " " + srcPort.ToString() + " " + dstPort.ToString() + "\r\n" + data.ToString() + "\r\n";
                    Console.WriteLine(sendNTP);
                }
            }
            // Закрываем pcap устройство
            //device.Close();
            //Console.WriteLine(" -- Capture stopped, device closed.");

        }
    }

}


            /*static void device_OnPacketArrival(object sender, CaptureEventArgs e)
            {
                // парсинг всего пакета
                Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                // получение только TCP пакета из всего фрейма
                var tcpPacket = TcpPacket.GetEncapsulated(packet);
                // получение только IP пакета из всего фрейма
                var ipPacket = IpPacket.GetEncapsulated(packet);
                if (tcpPacket != null && ipPacket != null)
                {
                    DateTime time = e.Packet.Timeval.Date;
                    int len = e.Packet.Data.Length;
                    Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
                                       time.Hour, time.Minute, time.Second,
                                       time.Millisecond, len);
                    //Console.WriteLine(e.Packet.ToString());
                    // IP адрес отправителя
                    var srcIp = ipPacket.SourceAddress.ToString();
                    //Console.WriteLine("srcIp="+ srcIp);
                    // IP адрес получателя
                    var dstIp = ipPacket.DestinationAddress.ToString();
                    //Console.WriteLine("dstIp=" + dstIp);
                    // порт отправителя
                    var srcPort = tcpPacket.SourcePort.ToString();
                    //Console.WriteLine("srcPort=" + srcPort);
                    // порт получателя
                    var dstPort = tcpPacket.DestinationPort.ToString();
                    //Console.WriteLine("dstPost=" + dstPort);
                    // данные пакета
                    var data =BitConverter.ToString(e.Packet.Data);
                    //Console.WriteLine("data=" + data);
                    string sendNTP = srcIp.ToString() + " " + dstIp.ToString() + " " + srcPort.ToString() + " " + dstPort.ToString() + " " + data.ToString();
                    Console.WriteLine(sendNTP);
                }
            }*/
