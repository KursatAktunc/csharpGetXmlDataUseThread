using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace VizeOdev
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static Thread thread1;

        string[] lines =
        {
            "P_NAME", "P_MFGR", "P_BRAND","P_TYPE","P_SIZE","P_CONTAINER","P_RETAILPRICE","P_COMMENT"
        };
        string[] dataNetwork = new string[8];
        string[] dataText = new string[8];

        private void Form1_Load(object sender, EventArgs e)
        {
            thread1 = new Thread(() =>
            {
                while (true)
                {
                    fetchXmlData();
                    dosyadanOku();
                    compareAndWriteData();
                    Thread.Sleep(100); //15dk da bir verileri kontrol eder
                }
            });
            thread1.Start();
        }

        void dosyadanOku()
        {
            if (File.Exists(@"WriteLines.txt"))
            {
                string[] textLine = File.ReadAllLines(@"WriteLines.txt");
                int counter = 0;

                foreach (string line in textLine)
                {
                    dataText[counter] = line;
                    counter++;
                }
            }
        }

        void fetchXmlData()
        {
            try
            {
                string BASE_URL = "http://aiweb.cs.washington.edu/research/projects/xmltk/xmldata/data/tpc-h/part.xml";

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(BASE_URL);

                for (int i = 0; i < lines.Length; i++)
                {
                    dataNetwork[i] = xmlDoc.SelectSingleNode("table[@ID='part']/T/" + lines[i]).InnerXml;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.CompanyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        void compareAndWriteData()
        {
            bool areEqual = dataNetwork.SequenceEqual(dataText);
            if (!areEqual)
            {
                MessageBox.Show("Eşleşmedi Yeni Kayıt Yapılıyor", "deneme", MessageBoxButtons.OK);
                File.WriteAllLines("WriteLines.txt", dataNetwork);

                listBox1.BeginInvoke(new Action(() =>
                {
                    listBox1.Items.Clear();
                    foreach (var item in dataNetwork)
                    {
                        listBox1.Items.Add(item);
                    }
                }));
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread1.Abort();
            Environment.Exit(1);
            Application.Exit();
        }
    }
}