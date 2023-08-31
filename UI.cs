using System;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace bwMonitor
{
    public partial class UI : Form
    {
        //instance of netport
        netport netport = new netport();
        
        //settings for moving windows
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public UI()
        {
            
        InitializeComponent();

            //add interfaces to combobox
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                comboInterfaces.Items.Add(ni.Name);
                
            }
    }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (comboInterfaces.SelectedItem != null)
            {
                //timer settings
                timer1.Interval = 1000; // 1 second interval
                timer1.Enabled = true;

                //pass current traffic to object
                netport.intName = comboInterfaces.SelectedItem.ToString();
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {

                    if (ni.Name == netport.intName)
                    {
                        netport.prevUpload = ni.GetIPStatistics().BytesSent;
                        netport.prevDownload = ni.GetIPStatistics().BytesReceived;

                        //get ipv4 address
                        var ipProps = ni.GetIPProperties();
                        foreach (var ip in ipProps.UnicastAddresses)
                        {
                            netport.ipv4add = ip.Address.ToString();
                        }

                    }
                }

                //chart series
                chart1.Series.Clear();
                
                Series serieUpload = new Series("Upload");
                serieUpload.ChartType = SeriesChartType.SplineArea;
                serieUpload.Color = Color.FromArgb(95, Color.Orange);
                serieUpload.BorderColor = Color.DarkOrange;
                serieUpload.BorderWidth = 2;

                Series serieDownload = new Series("Download");
                serieDownload.ChartType = SeriesChartType.SplineArea;
                serieDownload.Color = Color.FromArgb(95, Color.DeepSkyBlue);
                serieDownload.BorderColor = Color.DodgerBlue;
                serieDownload.BorderWidth = 2;

                chart1.Series.Add(serieUpload);
                chart1.Series.Add(serieDownload);
            }
            
        }

        private void UI_Load(object sender, EventArgs e)
        {
            //clear txt labels
            label2.Text = " ";
            label3.Text = " ";
            label1.Text = " ";
            label5.Text = " ";
            label6.Text = " ";
            

            //form & chart color
            button1.BackColor = Color.FromArgb(30,30,30);
            BackColor = Color.FromArgb(30, 30, 30);
            chart1.ChartAreas[0].BackColor = Color.FromArgb(30, 30, 30);

            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(80, Color.Silver);
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(80, Color.Silver);

            chart1.ChartAreas[0].AxisX.LineColor = Color.Silver;
            chart1.ChartAreas[0].AxisY.LineColor = Color.Silver;

            chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Silver;
            chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Silver;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                
                if (ni.Name == netport.intName)
                {
                    //label texts
                    label2.Text = "Download: ";
                    label3.Text = "Upload: ";
                    label6.Text = "IP: " + netport.ipv4add;

                    //send current bytes to object
                    netport.bytesSent = ni.GetIPStatistics().BytesSent;
                    netport.bytesReceived = ni.GetIPStatistics().BytesReceived;

                    //call object functions
                    long download = netport.downloadbw();
                    long upload = netport.uploadbw();

                    //set time value to x axis
                    string xAxisTimeValue = DateTime.Now.ToString("HH:mm:ss");

                    //traffic text
                    if(download > 1000)
                    {
                        label1.Text = download/1000 + " Mbps";
                    } else
                    {
                        label1.Text = download + " kbps";
                    }
                    if (upload > 1000)
                    {
                        label5.Text = upload/1000 + " Mbps";
                    }
                    else
                    {
                        label5.Text = upload + " kbps";
                    }

                    //add chart points
                    chart1.Series["Download"].Points.AddXY(xAxisTimeValue, download);
                    chart1.Series["Upload"].Points.AddY(upload);
                    

                }
            }

        }

        //function to move window
        private void UI_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        //buttons settings
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.DarkRed;
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.Red;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(70, 70, 70);
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(40, 40, 40);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            easter easter = new easter();
            easter.ShowDialog();
        }
    }
}
