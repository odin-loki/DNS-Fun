using DnsClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DWorks
{
    //IDEA: Consider More Server Parralelism. Only using 256, have around 10,000 to use.
    //IDEA: look for DNS records in common places such as .gov and .org, generate combinations between 1 to 6 letters for acronyms and then DNS Query.
    /*  Example Large DNS Records:
        americorps = 2457
        europa.eu = 4727
        microsoft = 1716
        cnn.com = 2447
        paypal.com = 3093
        hsbc.com = 1366
        netflix.com = 1522
        defense.gov = 2695
        nasa.gov = 3407
        mit.edu = 2230
        ferc.gov = 5858
        usa.gov = 3387
     */

    /// <summary>
    /// Should take a day to Query the entire internet of DNS Records.
    /// </summary>
    public class DnsAnalytics
    {
        /// <summary>
        /// DnsAnalytics Internal Class for Threaded Data passing.
        /// </summary>
        private class Data
        {
            ///Default Constructor.
            public Data(uint lowerBound, uint upperBound, string address)
            {
                LowerBound = lowerBound;
                UpperBound = upperBound;
                SetClient(in address);
            }

            ///Set DNS Lookup Client. Threads can accept Nameserver change later.
            public void SetClient(in string address)
            {
                Client = new LookupClient(IPAddress.Parse(address))
                {
                    Timeout = TimeSpan.FromMilliseconds(20)
                };
            }

            ///Lower Bound of Parralell Loop.
            public uint LowerBound { get; set; }
            ///Upper Bound of Parralell Loop.
            public uint UpperBound { get; set; }
            ///Lookup Client.
            public LookupClient Client { get; set; }
        }

        ///Increment Counter for Threaded Number of DNS Servers Queried.
        private static long done = 0;

        ///Test size for the DNS Record to be larger than.
        private const int testSize = 2000;

        ///Name of File with Largest DNS Servers in it.
        private const string largestRecord = "largest.txt";

        ///Utility Methods Class.
        private readonly Utilitys util = new Utilitys();

        ///Global Root DNS Name Servers. Requests Max out at 8000 Sequential.
        private static IPAddress[] rootServers = { new IPAddress(3389791009), new IPAddress(3339259593),
            new IPAddress(3339148045), new IPAddress(3339146026), new IPAddress(3328294453), 
            new IPAddress(3324575748), new IPAddress(3238006401), new IPAddress(3234588170), 
            new IPAddress(3228574724), new IPAddress(3225059358), new IPAddress(3223622673), 
            new IPAddress(3223389196), new IPAddress(3221554673) };

        //Public DNS Name Servers list.
        private string[] nameServers;

        ///File IO Read and Write Lock for Threading.
        private static ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Thread Safe, Tests for DNS Record Size over 2000 Bytes and Writes to File.
        /// </summary>
        /// <param name="MessageSize">DNS Query Response Size</param>
        /// <param name="ip">IP4 Address</param>
        private void TestAndWrite(int MessageSize, ref string ip)
        {
            try
            {
                readWriteLock.EnterWriteLock();
                if (MessageSize > testSize)
                {
                    File.AppendAllText(largestRecord, ip + ", Size: " + MessageSize + '\n');
                }
            }
            catch (Exception E)
            {
                throw new Exception("Threading Error." + E.Message);
            }
            finally
            {
                readWriteLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Returns a Random Index from nameServers Array.
        /// </summary>
        /// <returns>Random int in nameServers array index range.</returns>
        private int RandIndex()
        {
            return new Random().Next(0, nameServers.Length - 1);
        }

        /// <summary>
        /// Single Threaded Find Largest DNS Record
        /// </summary>
        /// <param name="server">IP4 address of DNS Server to Query</param>
        public void FindLargestDnsRecord(string server)
        {
            LookupClient client = new LookupClient(IPAddress.Parse(server))
            {
                Timeout = TimeSpan.FromMilliseconds(20)
            };

            for (uint i = 0, maxValue = uint.MaxValue; i <= maxValue; i++)
            {
                string ip = util.FormatIP(i);
                IDnsQueryResponse response = client.Query(ip, QueryType.ANY, QueryClass.IN);
                TestAndWrite(response.MessageSize, ref ip);
                Console.Write('\r' + ip + ", " + (i / maxValue * 100) + "% Done.  ");
            }
        }

        /// <summary>
        /// Implicit Parralell Find Largest DNS Record. 
        /// </summary>
        /// <param name="server">IP4 address of DNS Server to Query</param>
        public void ParalellFindLargestDnsRecord(string server)
        {
            LookupClient client = new LookupClient(IPAddress.Parse(server))
            {
                Timeout = TimeSpan.FromMilliseconds(20)
            };

            uint maxValue = uint.MaxValue;
            Parallel.For(0, maxValue, async index =>
            {
                string ip = util.FormatIP((uint)index);
                IDnsQueryResponse response = await client.QueryAsync(ip, QueryType.ANY, QueryClass.IN);
                TestAndWrite(response.MessageSize, ref ip);
                Console.Write("\r" + (index / maxValue * 100) + "% Done.  ");
            });
        }

        /// <summary>
        /// Explicitly Threaded and Implicitly Paralell Find Largest DNS Record.
        /// </summary>
        /// <param name="_object">Data Object with DNS Query Client, Lower Bound and Upper Bound.</param>
        private void Threaded_FindLargestDnsRecord(object _object)
        {
            Data data = (Data)_object;
            Parallel.For(data.LowerBound, data.UpperBound, async index =>
            {
            START:
                try
                {
                    string ip = util.FormatIP(data.LowerBound);
                    IDnsQueryResponse response = await data.Client.QueryAsync(ip, QueryType.ANY, QueryClass.IN);
                    TestAndWrite(response.MessageSize, ref ip);
                    Interlocked.Increment(ref done);
                    Console.Write("\r" + (done / uint.MaxValue * 100) + "% Done.  ");
                }
                catch (DnsResponseException)
                {   //Sets a new client to query randomly and rebegins loop.
                    data.SetClient(in nameServers[RandIndex()]);
                    goto START;
                }
            });
        }

        /// <summary>
        /// Explicit Threaded Method.
        /// Creates 256 threads and Querys 256 DNS servers. 
        /// DNS Nameserver will block traffic, due to too many querys.
        /// Switches DNS Servers on timeout and continues work.
        /// </summary>
        public void Explicit_TFLD_Method(in string[] servers)
        {
            //Contemplate Thread Calculator. Eg. uint incrementRange = pow(2, 32) / desiredThreads;

            //Asign Nameservers to Class Global Variable.
            nameServers = servers;

            //Create threadlist
            List<Thread> threads = new List<Thread>() { };

            //Add Threads to threadlist
            for (int i = 0; i < 256; i++)
            {
                threads.Add(new Thread(Threaded_FindLargestDnsRecord));
                Console.WriteLine('\r' + "Thread " + i + " created.");
            }

            //Start all threads in threadlist. No need to join.
            for (uint i = 0, j = 0; j < 256; i += 16777216, j++)
            {
                Console.WriteLine('\r' + "Thread " + j + " Started.");
                threads[(int)j].Start(new Data(i, i + 16777216, servers[j]));
            }
        }

        /// <summary>
        /// ThreadPool_TFLD_ThreadedMethod Method. 
        /// Starts Threads on Thread Pool and waits for timeout.
        /// Begins with first 256 DNS Name Servers passed.
        /// Switches DNS Servers on timeout and continues work.
        /// No need for a Thread Calculator.
        /// </summary>
        /// <param name="servers">DNS Nameservers to Query</param>
        public void ThreadPool_TFLD_Method(in string[] servers)
        {
            //Asign Nameservers to Class Global Variable.
            nameServers = servers;

            //Send all Threads to Threadpool with data.
            for (uint i = 0, j = 0; j < 256; i += 16777216, j++)
            {
                Console.WriteLine('\r' + "Thread " + j + " Started.");
                ThreadPool.QueueUserWorkItem(Threaded_FindLargestDnsRecord, new Data(i, i + 16777216, servers[j]));
            }    
        }

    }
}
