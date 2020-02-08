using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Data;
using DnsClient;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using DnsClient.Protocol;

namespace dns
{
    class Program
    {
        static string nameServertxt = "nameserver.txt";
        static string recipienttxt = "recipient.txt";
        static string largestRecord = "largest.txt";
        static readonly IPAddress[] rootServers = { IPAddress.Parse("198.41.0.4"), IPAddress.Parse("199.9.14.201"), IPAddress.Parse("192.33.4.12"), IPAddress.Parse("199.7.91.13"), IPAddress.Parse("192.203.230.10"), IPAddress.Parse("192.5.5.241"), IPAddress.Parse("192.112.36.4"), IPAddress.Parse("198.97.190.53"), IPAddress.Parse("192.36.148.17"), IPAddress.Parse("192.58.128.30"), IPAddress.Parse("193.0.14.129"), IPAddress.Parse("199.7.83.42"), IPAddress.Parse("202.12.27.33") };

        public static string[] GetData(string filename)
        {
            if (File.Exists(filename))
            {
                Regex regex = new Regex("^(?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\\.){3}");
                List<string> temp = File.ReadAllLines(filename).ToList();
                return temp.Where(x => regex.IsMatch(x)).ToList().ToArray();
            }
            else
            {
                throw new Exception("File not found.");
            }
        }

        public static void Download_DnsServer()
        {
            if (!File.Exists(nameServertxt))
            {
                WebClient client = new WebClient();
                string data = client.DownloadString("https://public-dns.info/nameservers.txt");
                client.Dispose();
                File.WriteAllText(nameServertxt, data);

            }
        }

        public static void FindRecipients()
        {
            if (!File.Exists(recipienttxt))
            {
                throw new Exception("Create Recipient File.");
            }
        }

        public static uint ParseIP(string ip)
        {
            byte[] b = ip.Split('.').Select(s => Byte.Parse(s)).ToArray();
            if (BitConverter.IsLittleEndian) Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
        }

        public static string FormatIP(uint ip)
        {
            byte[] b = BitConverter.GetBytes(ip);
            if (BitConverter.IsLittleEndian) Array.Reverse(b);
            return String.Join(".", b.Select(n => n.ToString()));
        }

        public static void QueryAllServers()
        {
            Download_DnsServer();
            string[] dnsServer = GetData(nameServertxt);
            List<IPAddress> servers = new List<IPAddress>() { };

            for (int i = 0; i < dnsServer.Length; i++)
            {
                servers.Add(IPAddress.Parse(dnsServer[i]));
            }
            //FindLargestServer(servers.ToArray());
        }
        public static void FindLargestServer()
        {
            LookupClient client = new LookupClient(IPAddress.Parse("1.1.1.1"))
            {
                Timeout = TimeSpan.FromMilliseconds(20)
            };

            for (uint i = 0, j = uint.MaxValue; i <= j; i++)
            {
                int size = 0;
                IDnsQueryResponse response = client.Query(FormatIP(i), QueryType.ANY, QueryClass.IN);
                foreach (DnsResourceRecord item in response.AllRecords)
                {
                    size += item.RawDataLength;
                }

                if (size > 500)
                {
                    File.AppendAllText(largestRecord, (response.AllRecords.First().DomainName.ToString() + ", Size: " + size + '\n'));
                }
                Console.Write('\r' + FormatIP(i) + " ");
            }
        }

        public class Data
        {
            public Data(uint lowerBound, uint upperBound)
            {
                this.lowerBound = lowerBound;
                this.upperBound = upperBound;
            }

            public uint lowerBound { get; set; }
            public uint upperBound { get; set; }
        }


        public static async void ThreadedFindLargestServer(object obj)
        {
            Data data = (Data)obj;

            LookupClient client = new LookupClient(IPAddress.Parse("1.1.1.1"))
            {
                Timeout = TimeSpan.FromMilliseconds(20)
            };

            Parallel.For(data.lowerBound, data.upperBound, async index =>
            {
                int size = 0;
                IDnsQueryResponse response = await client.QueryAsync(FormatIP(data.lowerBound), QueryType.ANY, QueryClass.IN);
                foreach (DnsResourceRecord item in response.AllRecords)
                {
                    size += item.RawDataLength;
                }

                if (size > 500)
                {
                    File.AppendAllText(largestRecord, (response.AllRecords.First().DomainName.ToString() + ", Size: " + size + '\n'));
                }
                Interlocked.Increment(ref done);
                Console.Write("\r" + ((done / uint.MaxValue) * 100) + "% Done.  ");
            });
        }

        public static void Paralell()
        {
            List<Thread> threads = new List<Thread>() { };

            for (int i = 0; i < 256; i++)
            {
                threads.Add(new Thread(ThreadedFindLargestServer));
                Console.WriteLine('\r' + "Thread " + i + " created.");
            }

            for (uint i = 0; i < 256; i+=16777216)
            {
                Console.WriteLine('\r' + "Thread " + i + " Started.");
                threads[(int)i].Start(new Data(i, (i+16777216)));
            }

            for (uint i = 0; i < 256; i++)
            {
                threads[(int)i].Join();
                Console.WriteLine('\r' + "Thread " + i + " Joined.");
            }
        }

        static long done = 0;

        static void Main(string[] args)
        {

            Paralell();

            //FindLargestServer();

            //string[] dnsServer = GetData(nameServertxt);
            //string[] recServer = GetData(recipienttxt);
            //string[] largestServer = GetData(largestRecord);

            ////Socket sckt = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Raw);



            //LookupClient client = new LookupClient();
            //string hostName = await client.GetHostNameAsync(IPAddress.Parse("8.8.8.8"));

            //DnsString query; string quest;

            ////Get The DNS Host Name.
            //IDnsQueryResponse response1 = client.QueryServerReverse();

            ////Make DNS Query
            //DnsQuestion question = new DnsQuestion(quest, QueryType.ANY, QueryClass.IN);

            ////Query the DNS Record Size, 
            //IDnsQueryResponse response = client.QueryServer();

            //Console.WriteLine("Hello World!");
        }
    }
}
