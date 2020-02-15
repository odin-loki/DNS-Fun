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

namespace DWorks
{
    class Program
    {
        private static readonly DnsAnalytics analytics = new DnsAnalytics();
        private static readonly Utilitys utils = new Utilitys();

        static void Main(/*string[] args*/)
        {
            //analytics.FindLargestDnsRecord("1.1.1.1");
            //analytics.ParalellFindLargestDnsRecord("8.8.8.8");
            //analytics.Explicit_TFLD_Method(utils.GetPublicNameserversList());
            analytics.ThreadPool_TFLD_Method(utils.GetPublicNameserversList());
        }
    }
}
