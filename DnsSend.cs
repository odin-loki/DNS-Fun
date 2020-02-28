using DnsClient;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace DWorks
{
    public class DnsSend
    {
        private readonly Utilitys utils = new Utilitys();

        /// <summary>
        /// Creates a DNS Query String with ANY Type and In Class.
        /// </summary>
        /// <param name="server">Server to Query</param>
        /// <returns>DNS Query String</returns>
        private string CreateDNSQuery(in string server)
        {
            return new DnsQuestion(server, QueryType.ANY, QueryClass.IN).ToString();
        }


        //Use this internal Class. https://github.com/MichaCo/DnsClient.NET/blob/dev/src/DnsClient/DnsUdpMessageHandler.cs 

        //Use this class DnsRequestMessage to make DnsRequestMessage(DnsRequestHeader header, DnsQuestion question)

        public void SendData()
        {
            string[] dnsServer = utils.GetPublicNameserversList();
            string[] recServer = utils.GetRecipientList();
            string dnsQuestion = CreateDNSQuery("ferc.gov");

            Socket sckt = new Socket(AddressFamily.InterNetwork, SocketType.Raw, System.Net.Sockets.ProtocolType.Raw);
            sckt.Bind(new IPEndPoint("dest add", "dest prt"));
            sckt.SendTo("packet", new IPEndPoint("src add", "src port"));
        }

    }
}
