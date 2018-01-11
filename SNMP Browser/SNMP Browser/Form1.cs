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
    public partial class Form1 : Form
    {
        private TreeNode m_OldSelectNode;
        public Form1()
        {
            InitializeComponent();
            imageList1.Images.Add(Properties.Resources.Folder);
            imageList1.Images.Add(Properties.Resources.key);
            imageList1.Images.Add(Properties.Resources.entry);
            imageList1.Images.Add(Properties.Resources.leaf);
            imageList1.Images.Add(Properties.Resources.paper);
            imageList1.Images.Add(Properties.Resources.table);
            treeView1.ImageList = imageList1;
        }

        OpenFileDialog mibFileDialog = new OpenFileDialog();

        private void loadMIBFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mibFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = mibFileDialog.SafeFileName;
                Program.ParseMIBFile(mibFileDialog.SafeFileName);
                List<TreeNode> treeNodeList = new List<TreeNode>();

                int newObjectLength = 0;
                foreach (KeyValuePair<string, string> kvp in Program.mibObjects)
                {
                    if (kvp.Key.Equals("iso.org.dod.internet.mgmt"))
                    {
                        treeView1.Nodes.Add(kvp.Key);
                        treeNodeList.Add(treeView1.Nodes[0]);
                    }
                    else
                    {
                        Program.mibObjectsLevels.TryGetValue(kvp.Key, out newObjectLength);
                        if (treeNodeList.Count > newObjectLength)
                        {
                            Console.WriteLine(kvp.Key);
                            treeNodeList[newObjectLength - 1].Nodes.Add(kvp.Key);
                            treeNodeList[newObjectLength] = treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1];
                            treeNodeList[newObjectLength].ImageIndex = 1;
                        }
                        else
                        {
                            treeNodeList[newObjectLength - 1].Nodes.Add(kvp.Key);
                            treeNodeList.Add(treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1]);
                        }
                        string option;
                        Program.mibObjectsTypes.TryGetValue(kvp.Key, out option);
                        switch (option)
                        {
                            case "F":
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].ImageIndex = 0;
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].SelectedImageIndex = 0;
                                break;
                            case "L":
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].ImageIndex = 3;
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].SelectedImageIndex = 3;
                                break;
                            case "P":
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].ImageIndex = 4;
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].SelectedImageIndex = 4;
                                break;
                            case "E":
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].ImageIndex = 2;
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].SelectedImageIndex = 2;
                                break;
                            case "T":
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].ImageIndex = 5;
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].SelectedImageIndex = 5;
                                break;
                            case "K":
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].ImageIndex = 1;
                                treeNodeList[newObjectLength - 1].Nodes[treeNodeList[newObjectLength - 1].Nodes.Count - 1].SelectedImageIndex = 1;
                                break;
                        }
                    }
                }
            }
        }

        /*
        private void initiallyFillSelectedObjectGrid()
        {
            dataGridView2.Rows.Add();
            dataGridView2.Rows[0].Cells[0].Value = "Name";

            dataGridView2.Rows.Add();
            dataGridView2.Rows[1].Cells[0].Value = "OID";

            dataGridView2.Rows.Add();
            dataGridView2.Rows[2].Cells[0].Value = "MIB";

            dataGridView2.Rows.Add();
            dataGridView2.Rows[3].Cells[0].Value = "Syntax";

            dataGridView2.Rows.Add();
            dataGridView2.Rows[4].Cells[0].Value = "Access";

            dataGridView2.Rows.Add();
            dataGridView2.Rows[5].Cells[0].Value = "Status";

            dataGridView2.Rows.Add();
            dataGridView2.Rows[6].Cells[0].Value = "DefVal";

            dataGridView2.Rows.Add();
            dataGridView2.Rows[7].Cells[0].Value = "Indexes";

            dataGridView2.Rows.Add();
            dataGridView2.Rows[8].Cells[0].Value = "Descr";
        }
        */

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string text;
            if (Program.mibObjects.TryGetValue(treeView1.SelectedNode.Text, out text))
                textBox2.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Equals("Get"))
            {
                List<string> oids = new List<string>();
                oids.Add(textBox2.Text);
                Dictionary<Oid, AsnType> results = Program.getResult(oids);
                if (results != null)
                {
                    foreach (KeyValuePair<Oid, AsnType> kvp in results)
                    {
                        string value = "." + kvp.Key.ToString();
                        var newKey = Program.mibObjects.FirstOrDefault(x => x.Value == value).Key;
                        int n = dataGridView1.Rows.Add();
                        dataGridView1.Rows[n].Cells[0].Value = newKey;
                        dataGridView1.Rows[n].Cells[1].Value = kvp.Value.ToString();
                        dataGridView1.Rows[n].Cells[2].Value = SnmpConstants.GetTypeName(kvp.Value.Type);
                        dataGridView1.Rows[n].Cells[3].Value = Program.address + ":" + Program.port;
                    }
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
                }
            }

            else if (comboBox1.Text.Equals("GetNext"))
            {
                List<string> oids = new List<string>();
                oids.Add(textBox2.Text);
                Dictionary<Oid, AsnType> results = Program.getNextResult(oids);
                if (results != null)
                {
                    foreach (KeyValuePair<Oid, AsnType> kvp in results)
                    {
                        string value = "." + kvp.Key.ToString();
                        var newKey = Program.mibObjects.FirstOrDefault(x => x.Value == value).Key;
                        int n = dataGridView1.Rows.Add();
                        dataGridView1.Rows[n].Cells[0].Value = newKey;
                        dataGridView1.Rows[n].Cells[1].Value = kvp.Value.ToString();
                        dataGridView1.Rows[n].Cells[2].Value = SnmpConstants.GetTypeName(kvp.Value.Type);
                        dataGridView1.Rows[n].Cells[3].Value = Program.address + ":" + Program.port;

                    }
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
                }
            }

            else if (comboBox1.Text.Equals("GetTable"))
            {
                GetTable(Program.address, Program.community, textBox2.Text, dataGridView3);
                tabControl1.SelectedTab = tabPage3;
            }

            else if (comboBox1.Text.Equals("Monitor"))
            {
                string name = null;
                string type = null;
                string address = null;
                List<string> oids = new List<string>();
                oids.Add(textBox2.Text);
                Dictionary<Oid, AsnType> results = Program.getResult(oids);
                
                if (results != null)
                {
                    foreach (KeyValuePair<Oid, AsnType> kvp in results)
                    {
                        string value = "." + kvp.Key.ToString();
                        var newKey = Program.mibObjects.FirstOrDefault(x => x.Value == value).Key;
                        name = newKey;
                        type = SnmpConstants.GetTypeName(kvp.Value.Type);
                        address = Program.address + ":" + Program.port;
                    }
                    new MonitorForm(name, type, address).Show();
                }
                else
                {
                    MessageBox.Show("No data to monitor");
                    return;
                }
                
            }

            if (dataGridView1.SelectedCells.Count > 0)
            {
                int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];

                string a;
                Program.mibObjects.TryGetValue(Convert.ToString(selectedRow.Cells[0].Value), out a);
                //MessageBox.Show(Convert.ToString(selectedRow.Cells[0].Value));
                textBox2.Text = a;

            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {
            // ImageList index value for the hover image.
            button2.ImageIndex = 1;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            // ImageList index value for the normal image.
            button2.ImageIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.trapListener.trapListenerThread = new Thread(() => Program.trapListener.initializeTrapListener(dataGridView2, richTextBox1));
            Program.trapListener.trapListenerThread.Start();
            label1.Text = "Listening";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Program.trapListener.trapListenerThread != null)
            {
                Program.trapListener.trapListenerThread.Abort();
                Program.trapListener.stopListening();
            }
            label1.Text = "Stopped";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            richTextBox1.Clear();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];

                string a;
                Program.mibObjects.TryGetValue(Convert.ToString(selectedRow.Cells[0].Value), out a);
                //MessageBox.Show(Convert.ToString(selectedRow.Cells[0].Value));
                textBox2.Text = a;

            }
        }

        public static void GetTable(string host, string community, string oid, DataGridView table)
        {
            string type;
            Program.mibObjectsTypes.TryGetValue(Program.mibObjects.FirstOrDefault(x => x.Value == oid).Key, out type);
            if (type != "T")
            {
                MessageBox.Show("Selected position is not a table!");
                return;
            }

            table.Rows.Clear();
            table.Refresh();

            Dictionary<String, Dictionary<uint, AsnType>> result = new Dictionary<String, Dictionary<uint, AsnType>>();
            List<uint> tableColumns = new List<uint>();
            List<string> tableRows = new List<string>();

            AgentParameters param;
            IpAddress peer;

            try
            {
                 param = new AgentParameters(SnmpVersion.Ver2, new OctetString(community));
                 peer = new IpAddress(host);
            }
            catch (Exception e)
            {
                MessageBox.Show("Invalid IP/port or community settings");
                return;
            }
            

            if (!peer.Valid)
            {
                MessageBox.Show("Unable to resolve name or error in address for peer: {0}", host);
                return;
            }

            UdpTarget target = new UdpTarget((System.Net.IPAddress)peer);
            Oid startOid = new Oid(oid);
            startOid.Add(1);
            Pdu getNextPdu = Pdu.GetNextPdu();
            Oid curOid = (Oid)startOid.Clone();
            List<string> columnNames = new List<string>();

            string searchOid = "." + startOid.ToString();
            foreach (KeyValuePair<string, string> kvp in Program.mibObjects)
            {
                if (kvp.Value.Contains(searchOid) && !kvp.Value.Equals(searchOid))
                    columnNames.Add(kvp.Key);
            }

            while (startOid.IsRootOf(curOid))
            {
                SnmpPacket res = null;
                try
                {
                    res = target.Request(getNextPdu, param);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Request failed: {0}", ex.Message);
                    target.Close();
                    return;
                }

                if (res.Pdu.ErrorStatus != 0)
                {
                    MessageBox.Show("SNMP agent returned error " + res.Pdu.ErrorStatus + " for request Vb index " + res.Pdu.ErrorIndex);
                    target.Close();
                    return;
                }

                foreach (Vb v in res.Pdu.VbList)
                {
                    curOid = (Oid)v.Oid.Clone();
                    if (startOid.IsRootOf(v.Oid))
                    {
                        uint[] childOids = Oid.GetChildIdentifiers(startOid, v.Oid);
                        uint[] instance = new uint[childOids.Length - 1];
                        Array.Copy(childOids, 1, instance, 0, childOids.Length - 1);
                        String strInst = InstanceToString(instance);
                        uint column = childOids[0];
                        if (!tableColumns.Contains(column))
                            tableColumns.Add(column);
                        if (result.ContainsKey(strInst))
                        {
                            result[strInst][column] = (AsnType)v.Value.Clone();
                        }
                        else
                        {
                            result[strInst] = new Dictionary<uint, AsnType>();
                            result[strInst][column] = (AsnType)v.Value.Clone();
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (startOid.IsRootOf(curOid))
                {
                    getNextPdu.VbList.Clear();
                    getNextPdu.VbList.Add(curOid);
                }
            }
            target.Close();

            if (result.Count <= 0)
            {
                MessageBox.Show("No results returned.");
            }
            else
            {
                table.ColumnCount = tableColumns.Count + 1;
                table.Columns[0].Name = "Instance";
                for (int i = 0; i < tableColumns.Count; i++)
                {
                    table.Columns[i + 1].Name = columnNames[i];
                    //table.Columns[i + 1].Name = "Column id " + tableColumns[i];
                }
                foreach (KeyValuePair<string, Dictionary<uint, AsnType>> kvp in result)
                {
                    tableRows.Add(kvp.Key);
                    foreach (uint column in tableColumns)
                    {
                        if (kvp.Value.ContainsKey(column))
                        {
                            tableRows.Add(kvp.Value[column].ToString());
                        }
                        else
                        {
                            tableRows.Add("");
                        }
                    }
                    table.Rows.Add(tableRows.ToArray());
                    tableRows.Clear();
                }
            }
        }

        public static string InstanceToString(uint[] instance)
        {
            StringBuilder str = new StringBuilder();
            foreach (uint v in instance)
            {
                if (str.Length == 0)
                    str.Append(v);
                else
                    str.AppendFormat(".{0}", v);
            }
            return str.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView3.Rows.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }
    }
}
