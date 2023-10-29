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
            int[] currentParts = { Int32.Parse(currentTemp[0]), Int32.Parse(currentTemp[1]), Int32.Parse(currentTemp[2]), Int32.Parse(currentTemp[3]) };

            //debugging.......
            Console.WriteLine(currentPing);
            Console.WriteLine(currentCIDR);

            for (int i = 0; i < currentParts.Length; i++)
                Console.Write(currentParts[i] + ", ");
            //................

            string currentStartIP = "";

            if(currentCIDR > 0 && currentCIDR < 36)
            {
                string maskedPart = "";
                int currentMaskedPart = 0;
                if (currentCIDR < 8)
                {
                    maskedPart = Convert.ToString(currentParts[0], 2).PadLeft(8, '0');
                    maskedPart = maskedPart.Substring(0, currentCIDR % 8).PadRight(8, '0');
                    currentMaskedPart = Convert.ToInt32(maskedPart, 2);
                    currentStartIP = currentMaskedPart + ".0.0.0";
                }
                else if (currentCIDR >= 8 && currentCIDR < 16)
                {
                    maskedPart = Convert.ToString(currentParts[1], 2).PadLeft(8, '0');
                    maskedPart = maskedPart.Substring(0, currentCIDR % 8).PadRight(8, '0');
                    currentMaskedPart = Convert.ToInt32(maskedPart, 2);
                    currentStartIP = currentParts[0] + "." + currentMaskedPart + ".0.0";
                }
                else if (currentCIDR >= 16 && currentCIDR < 24)
                {
                    maskedPart = Convert.ToString(currentParts[2], 2).PadLeft(8, '0');
                    maskedPart = maskedPart.Substring(0, currentCIDR % 8).PadRight(8, '0');
                    currentMaskedPart = Convert.ToInt32(maskedPart, 2);
                    currentStartIP = currentParts[0] + "." + currentParts[1] + "." + currentMaskedPart + ".0";
                }
                else
                {
                    maskedPart = Convert.ToString(currentParts[3], 2).PadLeft(8, '0');
                    maskedPart = maskedPart.Substring(0, currentCIDR % 8).PadRight(8, '0');
                    currentMaskedPart = Convert.ToInt32(maskedPart, 2);
                    currentStartIP = currentParts[0] + "." + currentParts[1] + "." + currentParts[2] + "." + currentMaskedPart;
                }
                Console.WriteLine("\nSTART IP: " + currentStartIP);
            }
            else
            {
                Console.WriteLine("ERROR: CIDR OUT OF BOUNDS");
            }
            
            for (int i = 0; i < 254; i++)
            {
                currentPing = input_ip.Text;

                curr = new Ping();
                reply = curr.Send(currentPing + i.ToString());

                //PLS MAN FIX KI TF JE TU
                this.BeginInvoke((Action)delegate ()
                {
                    //counter
                    counter = "current ip: " + currentPing + i.ToString();
                    curr_ip.Text = counter;
                });
                //!!!

                if (reply.Status == IPStatus.Success)
                {
                    try
                    {
                        ip_address = IPAddress.Parse(currentPing + i.ToString());
                        host = Dns.GetHostEntry(ip_address);
                        name = host.HostName;

                        this.BeginInvoke((Action)delegate ()
                        {
                            int n = dgv.Rows.Count;
                            dgv.Rows.Add();
                            
                            dgv.Rows[n].Cells[0].Value = currentPing + i.ToString();
                            dgv.Rows[n].Cells[1].Value = name;
                            dgv.Rows[n].Cells[2].Value = "Active";                           
                        });
                    }
                    catch (Exception exception)
                    {

                    }
                }
            }
            MessageBox.Show("Subnet scan completed");
        }
    }
}
