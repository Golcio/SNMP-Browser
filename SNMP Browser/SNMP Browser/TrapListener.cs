using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNMP_Browser
{
    class TrapListener
    {
        Socket socket;
        IPEndPoint ipep;
        bool run;
        public Thread trapListenerThread;
        public TrapListener()
        {
            // Construct a socket and bind it to the trap manager port 162 
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ipep = new IPEndPoint(IPAddress.Any, 162);
            EndPoint ep = (EndPoint)ipep;
            socket.Bind(ep);
            // Disable timeout processing. Just block until packet is received 
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 0);
            run = true;
        }

        public void stopListening()
        {
            run = false;
        }

        public void writeToRtb(string text, RichTextBox rtb)
        {
            rtb.Invoke(new Action(delegate () { rtb.AppendText(text); }));
        }

        public void initializeTrapListener(DataGridView table, RichTextBox rtb)
        {

            run = true;
            int inlen = -1;
            while (run)
            {
                byte[] indata = new byte[16 * 1024];
                // 16KB receive buffer int inlen = 0;
                IPEndPoint peer = new IPEndPoint(IPAddress.Any, 0);
                EndPoint inep = (EndPoint)peer;
                try
                {
                    inlen = socket.ReceiveFrom(indata, ref inep);
                }
                catch (Exception ex) { }

                if (inlen > 0)
                {
                    int ver = SnmpPacket.GetProtocolVersion(indata, inlen);
                    if (ver == (int)SnmpVersion.Ver1)
                    {
                        // Parse SNMP Version 1 TRAP packet 
                        SnmpV1TrapPacket pkt = new SnmpV1TrapPacket();
                        pkt.decode(indata, inlen);

                        int n = 0;
                        table.Invoke(new Action(delegate () { n = table.Rows.Add(); }));
                        string oid = "." + pkt.Pdu.Enterprise.ToString() + "." + pkt.Pdu.Specific.ToString();
                        var objectName = Program.mibObjects.FirstOrDefault(x => x.Value == oid).Key;
                        if (objectName != null)
                            table.Invoke(new Action(delegate () { table.Rows[n].Cells[0].Value = "Specific: " + pkt.Pdu.Specific + "; " + objectName; }));
                        else
                            table.Invoke(new Action(delegate () { table.Rows[n].Cells[0].Value = "Specific: " + pkt.Pdu.Specific + "; " + oid; }));
                        table.Invoke(new Action(delegate () { table.Rows[n].Cells[1].Value = pkt.Pdu.AgentAddress.ToString(); }));
                        table.Invoke(new Action(delegate () { table.Rows[n].Cells[2].Value = pkt.Pdu.TimeStamp.ToString(); }));
                        table.Invoke(new Action(delegate () { table.FirstDisplayedScrollingRowIndex = table.RowCount - 1; }));
                        table.Invoke(new Action(delegate () { table.Rows[table.Rows.Count - 1].Selected = true; }));

                        rtb.Invoke(new Action(delegate () { rtb.Clear(); }));
                        writeToRtb("Source: " + pkt.Pdu.AgentAddress.ToString() + "\n", rtb);
                        writeToRtb("Timestamp: " + pkt.Pdu.TimeStamp.ToString() + "\n", rtb);
                        writeToRtb("SNMP Version: " + pkt.Version + "\n", rtb);
                        writeToRtb("Enterprise: " + pkt.Pdu.Enterprise + "\n", rtb);
                        writeToRtb("Community: " + pkt.Community + "\n", rtb);
                        writeToRtb("Specific: " + pkt.Pdu.Specific + "\n", rtb);
                        writeToRtb("Generic: " + pkt.Pdu.Generic + "\n", rtb);
                        //writeToRtb("Description: " + "tu opis kij wie skad kij wie jak" + "\n", rtb);
                    }
                    else
                    {
                        /*
                        // Parse SNMP Version 2 TRAP packet 
                        SnmpV2Packet pkt = new SnmpV2Packet();
                        pkt.decode(indata, inlen);
                        Console.WriteLine("** SNMP Version 2 TRAP received from {0}:", inep.ToString());
                        if ((SnmpSharpNet.PduType)pkt.Pdu.Type != PduType.V2Trap)
                        {
                            Console.WriteLine("*** NOT an SNMPv2 trap ****");
                        }
                        else
                        {
                            Console.WriteLine("*** Community: {0}", pkt.Community.ToString());
                            Console.WriteLine("*** VarBind count: {0}", pkt.Pdu.VbList.Count);
                            Console.WriteLine("*** VarBind content:");
                            foreach (Vb v in pkt.Pdu.VbList)
                            {
                                Console.WriteLine("**** {0} {1}: {2}",
                                   v.Oid.ToString(), SnmpConstants.GetTypeName(v.Value.Type), v.Value.ToString());
                            }
                            Console.WriteLine("** End of SNMP Version 2 TRAP data.");
                        }
                        */
                    }
                }
                else
                {
                    //if (inlen == 0)
                        //Console.WriteLine("Zero length packet received.");
                }
            }
        }
    }
}
