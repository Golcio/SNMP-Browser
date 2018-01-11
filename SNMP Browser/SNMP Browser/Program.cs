using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNMP_Browser
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        /// 
        static Form1 form1;
        public static TrapListener trapListener;
        public static Dictionary<string, string> mibObjects = new Dictionary<string, string>();
        public static Dictionary<string, string> mibObjectsTypes = new Dictionary<string, string>();
        public static Dictionary<string, int> mibObjectsLevels = new Dictionary<string, int>();
        public static string community = null;
        public static string address = null;
        public static string port = null;

        [STAThread]
        static void Main()
        {
            trapListener = new TrapListener();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(form1 = new Form1());
        }

        public static Dictionary<Oid, AsnType> getResult(List<string> oids)
        {
            MessageBox.Show(oids[0]);
            SimpleSnmp snmp;
            try
            {
                snmp = new SimpleSnmp(address, community);
                if (!snmp.Valid)
                {
                    MessageBox.Show("SNMP agent host name/IP address is invalid.");
                    return null;
                }
                Dictionary<Oid, AsnType> result = snmp.Get(SnmpVersion.Ver1, oids.ToArray());
                if (result == null)
                {
                    MessageBox.Show("No results received.");
                    return null;
                }

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("SNMP agent host name/IP address is invalid.");
                return null;
            }
        }

        public static Dictionary<Oid, AsnType> getNextResult(List<string> oids)
        {
            SimpleSnmp snmp;
            try
            {
                snmp = new SimpleSnmp(address, community);
                if (!snmp.Valid)
                {
                    MessageBox.Show("SNMP agent host name/IP address is invalid.");
                    return null;
                }
                Dictionary<Oid, AsnType> result = snmp.GetNext(SnmpVersion.Ver1, oids.ToArray());
                if (result == null)
                {
                    MessageBox.Show("No results received.");
                    return null;
                }

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("SNMP agent host name/IP address is invalid.");
                return null;
            }
        }

        public static void ParseMIBFile(string file)
        {
            var fileName = System.Reflection.Assembly.GetExecutingAssembly().Location + "\\..\\..\\..\\Resources/" + file;
            try
            {
                var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    mibObjects.Add("iso.org.dod.internet.mgmt", ".1.3.6.1.2");
                    mibObjectsTypes.Add("iso.org.dod.internet.mgmt", "F");
                    mibObjectsLevels.Add("iso.org.dod.internet.mgmt", 0);
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] array = line.Split('#');
                        mibObjects.Add(array[2], array[1]);
                        Console.WriteLine(array[2]);
                        mibObjectsTypes.Add(array[2], array[0]);
                        mibObjectsLevels.Add(array[2], Int32.Parse(array[3]));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        
    }
}
