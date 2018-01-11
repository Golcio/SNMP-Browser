using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNMP_Browser
{
    public partial class MonitorForm : Form
    {
        Thread monitoringThread;
        string oid;
        public MonitorForm(string objectName, string type, string address)
        {
            InitializeComponent();
            this.textBox1.Text = objectName;
            this.textBox3.Text = type;
            this.textBox4.Text = address;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            monitoringThread = new Thread(() => startMonitoring());
            monitoringThread.Start();
        }

        private void startMonitoring()
        {
            oid = Program.mibObjects[textBox1.Text];
            while(true)
            {
                List<string> oids = new List<string>();
                oids.Add(oid);

                SimpleSnmp snmp;
                Dictionary<Oid, AsnType> results;
                try
                {
                    snmp = new SimpleSnmp(Program.address, Program.community);
                    if (!snmp.Valid)
                    {
                        MessageBox.Show("SNMP agent host name/IP address is invalid.");
                        return;
                    }
                    Dictionary<Oid, AsnType> result = snmp.Get(SnmpVersion.Ver1, oids.ToArray());
                    if (result == null)
                    {
                        MessageBox.Show("No results received.");
                        return;
                    }

                    results = result;
                }
                catch (Exception e)
                {
                    return;
                }

                if (results != null)
                {
                    foreach (KeyValuePair<Oid, AsnType> kvp in results)
                    {
                        textBox2.Invoke(new Action(delegate () { textBox2.Text = kvp.Value.ToString(); }));
                    }
                }
                Thread.Sleep(10);
            }
        }

        private void stopMonitoring()
        {
            if (monitoringThread != null)
            {
                monitoringThread.Abort();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stopMonitoring();
        }

        private void MonitorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopMonitoring();
        }
    }
}
