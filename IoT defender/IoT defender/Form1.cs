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
        private void button1_Click(object sender, EventArgs e)
        {
            bw.RunWorkerAsync();
        }
        private void Scan(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(100);

            string name = "";
            string counter = "";
            IPAddress ip_address;
            Ping curr;
            PingReply reply;
            IPHostEntry host;            

            string currentAddr = input_ip.Text;
            string[] currentTemp = currentAddr.Split('/');
            string currentPing = currentTemp[0];
            int currentCIDR = Int32.Parse(currentTemp[1]);

            IPAddress ip = IPAddress.Parse(currentPing);
            byte[] ipBytes = ip.GetAddressBytes();

            uint mask = ~(uint.MaxValue >> currentCIDR);
            byte[] maskBytes = BitConverter.GetBytes(mask);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(maskBytes);
            }

            byte[] networkAddress = new byte[ipBytes.Length];
            byte[] broadcastAddress = new byte[ipBytes.Length];

            for (int i = 0; i < ipBytes.Length; i++)
            {
                networkAddress[i] = (byte)(ipBytes[i] & maskBytes[i]);
                broadcastAddress[i] = (byte)(networkAddress[i] | ~maskBytes[i]);
            }

            IPAddress firstIP = new IPAddress(networkAddress);
            IPAddress lastIP = new IPAddress(broadcastAddress);

            //for debugging purposes.............
            Console.WriteLine("Start IP: " + firstIP.ToString());
            Console.WriteLine("End IP: " + lastIP.ToString());
            Console.WriteLine("CIDR: " + currentCIDR);
            //...................................

            byte[] ipBytesIterated = networkAddress.ToArray();
            while (!ipBytesIterated.SequenceEqual(broadcastAddress))
            {
                IPAddress currentIP = new IPAddress(ipBytesIterated);
                curr = new Ping();
                Console.WriteLine("Current IP: " + currentIP);
                reply = curr.Send(currentIP);

                //PLS MAN FIX KI TF JE TU
                this.BeginInvoke((Action)delegate ()
                {
                    //counter is behind by 1 and i don't know why
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
                        Console.WriteLine("ERROR: REPLY STATUS: " + exception.ToString());
                    }
                }
                IncrementIPAddressBytes(ref ipBytesIterated);
            }
            MessageBox.Show("Subnet scan completed");
        }
        //function for incrementing to next ip address
        private void IncrementIPAddressBytes(ref byte[] ipBytes)
        {
            for (int i = ipBytes.Length - 1; i >= 0; i--)
            {
                ipBytes[i]++;

                if (ipBytes[i] != 0) // exit loop in case of overflow
                {
                    break;
                }
            }
        }
    }
}
