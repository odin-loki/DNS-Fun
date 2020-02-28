//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace PacketGen
//{
//    public class IP4
//    {

///*  version = 4 bits
//*  IHL = 4 bits
//*  DSCP = 6 bits
//*  ECN = 2 bits
//*  Total Length = 16 bits
//*  Identification = 16 bits
//*  Flags = 3 bits
//*  Fragment Offset = 13 bits
//*  Time to Live = 8 bits
//*  Protocol = 8 bits
//*  Header Checksum = 16 bits
//*  Src IP = 32 bits
//*  Dest IP = 32 bits
//*/


//Consider borrowing Network Packets from  https://github.com/kyungyonglee/ipop/tree/master/src/NetworkPackets or MJsniffer

//        public IP4(BitArray version, BitArray iHL, BitArray dSCP, BitArray eCN, ushort totalLength, ushort identification, BitArray flags, BitArray fragmentsOffset, uint srcIP, uint destIP)
//        {
//            Version = version;
//            IHL = iHL;
//            DSCP = dSCP;
//            ECN = eCN;
//            TotalLength = totalLength;
//            Identification = identification;
//            Flags = flags;
//            FragmentsOffset = fragmentsOffset;
//            SrcIP = srcIP;
//            DestIP = destIP;
//        }

//        public IP4(BitArray version, BitArray iHL, BitArray dSCP, BitArray eCN, ushort totalLength, ushort identification, BitArray flags, BitArray fragmentsOffset, uint srcIP, uint destIP, byte[] options)
//        {
//            Version = version;
//            IHL = iHL;
//            DSCP = dSCP;
//            ECN = eCN;
//            TotalLength = totalLength;
//            Identification = identification;
//            Flags = flags;
//            FragmentsOffset = fragmentsOffset;
//            SrcIP = srcIP;
//            DestIP = destIP;
//            Options = options;
//        }

//        public byte Version { get; set; }
//        public byte IHL { get; set; }
//        public byte DSCP { get; set; }
//        public byte ECN { get; set; }
//        public ushort TotalLength { get; set; }
//        public ushort Identification { get; set; }
//        public byte Flags { get; set; }
//        public byte FragmentsOffset { get; set; }
//        public byte TimeToLive { get; set; }
//        public byte Protocol { get; set; }
//        public uint CheckSum { get; set; }
//        public uint SrcIP { get; set; }
//        public uint DestIP { get; set; }
//        public byte[] Options { get; set; } //Options Length 16


//        public ushort MakeFirstHalf()
//        {
//            uint version = 4 << 12;
//            uint IHL = 5 << 8;
//            uint DSCP_ECN = 0;
//            return (ushort)(version | IHL | DSCP_ECN);
//        }

//        //Calculate Length later

//        public uint MakeSecondLayer()
//        {
//            return 0;
//        }

//        public uint MakeThirdLayer()
//        {


//            return 0;
//        }

//        /// <summary>
//        /// Makes a IPv4 Default Version length.
//        /// </summary>
//        /// <returns>A byte with value 4</returns>
//        private byte MakeVersion()
//        {
//            return 4;
//        }

//        /// <summary>
//        /// Specifys the Length of the IPv4 Header in number of 32 bit words,
//        /// Minimum value is 5 and maximum is 6.
//        /// </summary>
//        /// <returns>A byte with default value 5 or 6</returns>
//        private byte MakeIHL()
//        {
//            if (Options.Length == 0)
//            {
//                return 5;
//            }
//            else
//            {
//                return 6;
//            }
//        }

//        /// <summary>
//        /// Makes a Differentiated Services Code Point (DSCP)
//        /// 64 Possible Values, only 14 used. Best value is 0.
//        /// Meaning Routine, Best Effort, with N/A Drop Probability.
//        /// </summary>
//        /// <returns>A byte with a default value of 0</returns>
//        private byte MakeDSCP()
//        {
//            return 0;
//        }

//        /// <summary>
//        /// Explicit Congestion Notification (ECN).
//        /// Only works when both servers agree on it.
//        /// Has four possible valies, from 2 bits.
//        /// </summary>
//        /// <returns>A byte with a default value of 0.</returns>
//        private byte MakeECN()
//        {
//            return 0;
//        }

//        /// <summary>
//        /// Speciifys the Total length of the Header and all the Data.
//        /// Runs as a Callback.
//        /// </summary>
//        /// <returns>Usigned 16 bit number</returns>
//        private ushort MakeTotalLength()
//        {
//            return 0;
//        }

//        /// <summary>
//        /// Packet Identifior. Defaults to Zero.
//        /// </summary>
//        /// <returns>Returns 0</returns>
//        private ushort MakeIdentification()
//        {
//            return 0;
//        }

//        /// <summary>
//        /// Controls Fragmentation if Packet oversize.
//        /// </summary>
//        /// <returns>Returns 0</returns>
//        private byte MakeFlags()
//        {
//            return 0;
//        }

//        /// <summary>
//        /// Controls Offset if Packet Oversize.
//        /// </summary>
//        /// <returns>Returns 0</returns>
//        private byte MakeFragmentsOffset()
//        {
//            return 0;
//        }

//        private byte MakeTTL()
//        {
//            return 255;
//        }

//        private byte MakeProtocol()
//        {
//            return 17;
//        }

//        private byte MakeHeaderChecksum()
//        {
//            //Callback.
//        }

//        private uint MakeIP(string ip)
//        {
//            byte[] b = ip.Split('.').Select(s => byte.Parse(s)).ToArray();
//            if (BitConverter.IsLittleEndian) Array.Reverse(b);
//            return BitConverter.ToUInt32(b, 0);
//        }

//        private byte[] MakeOptions()
//        {
//            return null;
//        }
//    }
//}
