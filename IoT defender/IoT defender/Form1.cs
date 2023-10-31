using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace IoT_defender
{
    public partial class mainPage : Form
    {
        public mainPage()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            bw.RunWorkerAsync();
        }
        private void Scan(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(100);
            string name = "";

            IPAddress ip_address;

            Ping curr;
            PingReply reply;
            IPHostEntry host;

            string counter = "";

            //legit se ful pahitri pinging naslovov če bereš text od labela sam enkrat pa storaš u string.
            //(prej se je vsakič accessalu value pa je tu zgleda "pažrišnu")
            string currentAddr = input_ip.Text;
            string[] currentTemp = currentAddr.Split('/');
            string currentPing = currentTemp[0];
            int currentCIDR = Int32.Parse(currentTemp[1]);

            currentTemp = currentPing.Split('.');
            //fyi current parts == octets, sam nism se spovnu prej te beside
            int[] currentParts = { Int32.Parse(currentTemp[0]), Int32.Parse(currentTemp[1]), Int32.Parse(currentTemp[2]), Int32.Parse(currentTemp[3]) };

            //debugging.......
            Console.WriteLine(currentPing);
            Console.WriteLine(currentCIDR);

            for (int i = 0; i < currentParts.Length; i++)
                Console.Write(currentParts[i] + ", ");
            //................

            string currentStartIP = "";
            int currentLastOctet = -1;

            int currentMaskedPart = 0;
            if (currentCIDR > 0 && currentCIDR < 36)
            {
                string maskedPart = "";
                if (currentCIDR < 8)
                {
                    maskedPart = Convert.ToString(currentParts[0], 2).PadLeft(8, '0');
                    maskedPart = maskedPart.Substring(0, currentCIDR % 8).PadRight(8, '0');
                    currentMaskedPart = Convert.ToInt32(maskedPart, 2);
                    currentStartIP = currentMaskedPart + ".0.0.0";
                    currentLastOctet = 0;
                }
                else if (currentCIDR >= 8 && currentCIDR < 16)
                {
                    maskedPart = Convert.ToString(currentParts[1], 2).PadLeft(8, '0');
                    maskedPart = maskedPart.Substring(0, currentCIDR % 8).PadRight(8, '0');
                    currentMaskedPart = Convert.ToInt32(maskedPart, 2);
                    currentStartIP = currentParts[0] + "." + currentMaskedPart + ".0.0";
                    currentLastOctet = 1;
                }
                else if (currentCIDR >= 16 && currentCIDR < 24)
                {
                    maskedPart = Convert.ToString(currentParts[2], 2).PadLeft(8, '0');
                    maskedPart = maskedPart.Substring(0, currentCIDR % 8).PadRight(8, '0');
                    currentMaskedPart = Convert.ToInt32(maskedPart, 2);
                    currentStartIP = currentParts[0] + "." + currentParts[1] + "." + currentMaskedPart + ".0";
                    currentLastOctet = 2;
                }
                else
                {
                    maskedPart = Convert.ToString(currentParts[3], 2).PadLeft(8, '0');
                    maskedPart = maskedPart.Substring(0, currentCIDR % 8).PadRight(8, '0');
                    currentMaskedPart = Convert.ToInt32(maskedPart, 2);
                    currentStartIP = currentParts[0] + "." + currentParts[1] + "." + currentParts[2] + "." + currentMaskedPart;
                    currentLastOctet = 3;
                }
                Console.WriteLine("\nSTART IP: " + currentStartIP);
            }
            else
            {
                Console.WriteLine("ERROR: CIDR OUT OF BOUNDS");
            }
            if(currentLastOctet == -1)
                Console.WriteLine("ERROR: INVALID OCTET");

            //todo: to gre u infinite loop u primeru invalid octeta
            //nism sure glede pogoja tbh
            //todo: fix tale da 254, nism zihr lih kaku bi moglu bit no
            IPAddress convertanIP = IPAddress.Parse(currentStartIP);
            uint prviIP = BitConverter.ToUInt32(convertanIP.GetAddressBytes(), 0);
            IPAddress convertanZadnIP = IPAddress.Parse("255.255.255.255");
            uint zadnIP = BitConverter.ToUInt32(convertanZadnIP.GetAddressBytes(), 0);

            //Array.Reverse(bytes);
            for (uint i = prviIP + 1; i <= zadnIP; i++)
            {
                byte[] bytes = BitConverter.GetBytes(i);


                if (i != prviIP + 1)
                {
                    //Array.Reverse(bytes);
                    Console.WriteLine("i indeed ni enaka prvi ip");
                }

                //if (BitConverter.IsLittleEndian)
                    //Array.Reverse(bytes);

                //currentPing = currentParts[0] + "." + currentParts[1] + "." + currentParts[2] + "." + i;

                IPAddress currentIP = new IPAddress(bytes);

                curr = new Ping();
                Console.WriteLine("Current IP: " + currentIP);
                Console.WriteLine("i: " + i);
                for (int k = 0; k < bytes.Length; k++)
                    Console.WriteLine(k + ": " + bytes[k]);
                reply = curr.Send(currentIP);

                

                //PLS MAN FIX KI TF JE TU
                this.BeginInvoke((Action)delegate ()
                {
                    //counter
                    counter = "current ip: " + currentIP;
                    curr_ip.Text = counter;
                });
                //!!!

                if (reply.Status == IPStatus.Success)
                {
                    try
                    {
                        ip_address = IPAddress.Parse(currentIP.ToString());
                        host = Dns.GetHostEntry(ip_address);
                        name = host.HostName;

                        this.BeginInvoke((Action)delegate ()
                        {
                            int n = dgv.Rows.Count;
                            dgv.Rows.Add();

                            dgv.Rows[n].Cells[0].Value = currentIP;
                            dgv.Rows[n].Cells[1].Value = name;
                            dgv.Rows[n].Cells[2].Value = "Active";
                        });
                    }
                    catch (Exception exception)
                    {

                    }
                }
                //Array.Reverse(bytes);
            }
            MessageBox.Show("Subnet scan completed");
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
