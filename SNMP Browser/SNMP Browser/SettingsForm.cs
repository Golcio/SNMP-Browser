using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNMP_Browser
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            if (Program.community != null)
                textBox1.Text = Program.community;
            if (Program.address != null)
                textBox2.Text = Program.address;
            if (Program.port != null)
                textBox3.Text = Program.port;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.community = textBox1.Text;
            Program.address = textBox2.Text;
            Program.port = textBox3.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
