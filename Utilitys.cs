using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DWorks
{
    public class Utilitys
    {
        ///Filename of Public DNS Servers.
        private const string nameServertxt = "nameserver.txt";

        ///Filename of Recipient DNS Servers
        private const string recipienttxt = "recipient.txt";

        /// <summary>
        /// IP4 Regex
        /// </summary>
        readonly Regex Ip4Regex = new Regex("^(?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\\.){3}");

        /// <summary>
        /// Downloads a list of IP4 DNS servers into memory.
        /// </summary>
        private void Download_DnsServer()
        {
            if (!File.Exists(nameServertxt))
            {
                WebClient client = new WebClient();
                string data = client.DownloadString("https://public-dns.info/nameservers.txt");
                client.Dispose();
                File.WriteAllText(nameServertxt, data);
            }
        }

        /// <summary>
        /// Gets all IP4 data from a textfile.
        /// </summary>
        /// <param name="filename">The filename to read data from.</param>
        /// <returns>Returns a string array of IP4 Data</returns>
        private string[] GetData(string filename)
        {
            START:
            if (File.Exists(filename))
            {
                List<string> temp = File.ReadAllLines(filename).ToList();
                return temp.Where(x => Ip4Regex.IsMatch(x)).ToList().ToArray();
            }
            else
            {
                if (filename == nameServertxt)
                {
                    Download_DnsServer();
                    goto START;
                }
                throw new Exception("File " + filename + " could not be found or downloaded.");
            }
        }

        /// <summary>
        /// Reads a list of Public IP4 DNS Servers into memory.
        /// </summary>
        /// <returns>Returns a IP4 List of Public DNS Servers</returns>
        public string[] GetPublicNameserversList()
        {
            return GetData(nameServertxt);
        }

        /// <summary>
        /// Reads a list of Recipient IP4 addresses into memory.
        /// </summary>
        /// <returns>Returns a list of IP4 Recipients</returns>
        public string[] GetRecipientList()
        {
            return GetData(recipienttxt);
        }

        /// <summary>
        /// Parses a IP4 string to IP4 32 bit number.
        /// </summary>
        /// <param name="ip">String IP4 address</param>
        /// <returns>Unsigned 32 bit integer IP4 Address</returns>
        public uint ParseIP(string ip)
        {
            byte[] b = ip.Split('.').Select(s => byte.Parse(s)).ToArray();
            if (BitConverter.IsLittleEndian) Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
        }

        /// <summary>
        /// Parses a IP4 32 bit number to a IP4 string.
        /// </summary>
        /// <param name="ip">Unsigned 32 bit IP4 Number</param>
        /// <returns>IP4 String</returns>
        public string FormatIP(uint ip)
        {
            byte[] b = BitConverter.GetBytes(ip);
            if (BitConverter.IsLittleEndian) Array.Reverse(b);
            return string.Join(".", b.Select(n => n.ToString()));
        }

    }
}
