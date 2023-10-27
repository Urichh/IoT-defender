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
    public partial class Form1 : Form
    {
        public Form1()
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

            for (int i = 0; i < 254; i++)
            {
                curr = new Ping();
                reply = curr.Send(textBox1.Text + i.ToString());

                if (reply.Status == IPStatus.Success)
                {
                    try
                    {
                        ip_address = IPAddress.Parse(textBox1.Text + i.ToString());
                        host = Dns.GetHostEntry(ip_address);
                        name = host.HostName;

                        this.BeginInvoke((Action)delegate ()
                        {
                            int n = dgv.Rows.Count;
                            dgv.Rows.Add();
                            
                            dgv.Rows[n].Cells[0].Value = textBox1.Text + i.ToString();
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
