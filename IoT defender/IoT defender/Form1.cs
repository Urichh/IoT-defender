using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(100);
            Ping ping;
            IPAddress addr;
            PingReply pingReply;
            IPHostEntry host;
            string name;

            Parallel.For(0, 254, (i, loopState) =>
            {
                    ping = new Ping();
                    pingReply = ping.Send(textBox1.Text + i.ToString());

                this.BeginInvoke((Action)delegate ()
                {
                    if (pingReply.Status == IPStatus.Success)
                    {
                        try
                        {
                            addr = IPAddress.Parse(textBox1.Text + i.ToString());
                            host = Dns.GetHostEntry(addr);
                            name = host.HostName;

                            dataGridView1.Rows.Add();
                            int nRowIndex = dataGridView1.Rows.Count - 1;
                            dataGridView1.Rows[nRowIndex].Cells[0].Value = textBox1.Text + i.ToString();
                            dataGridView1.Rows[nRowIndex].Cells[1].Value = name;
                            dataGridView1.Rows[nRowIndex].Cells[2].Value = "Active";
                        }
                        catch (Exception ex)
                        {
                            name = "?";
                        }
                    }
                });
            });
            MessageBox.Show("Scan completed");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
