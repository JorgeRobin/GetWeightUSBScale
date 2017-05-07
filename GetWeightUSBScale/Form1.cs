using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScaleReader;
using System.Threading;
using System.Net;
using System.IO;

namespace GetWeightUSBScale
{
    public partial class Form1 : Form
    {
        decimal? weight;
        bool? isStable;
        // local directory to write the weight (replace Username and Appname with yours)
        string writeTo = "C:/Users/yourUsername/AppData/yourAppname";
        string weightId = "";
        // FTP address to upload the weight 
        string ftpSite = "ftp://" + "www.yoursite.com/yourdirectory";
        string ftpUsername = "YourFTPUsername";
        string ftpPassword = "YourFTPPassword";

        USBScale s = new USBScale();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                writeTo = fbd.SelectedPath;
            }
            textBox1.Text = writeTo;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (s.IsConnected)
            {

                s.GetWeight(out weight, out isStable);
                //s.DebugScaleData();
                s.Disconnect();
                // default to kgs
                string unit = "Kgs";
                if (Convert.ToInt16(s.ScaleWeightUnits) == 12) unit = "Lbs";
                //Console.WriteLine("Weight: {0:0.00} " + unit, weight);
                string json = "{\"weight\":" + "\"" + weight + "\", \"unit\":\"" + unit + "\"}";
                Console.WriteLine(writeTo + weightId);
                System.IO.File.WriteAllText(writeTo + "/" + weightId, json);
                label2.Text = "logging " + json;
                label3.Text = "Scale is connected " + weightId;
                // write to server
                writeToFTP();
                Thread.Sleep(1000);
            } else
            {
                label2.Text = "not logging (verify if scale is connected)";
                label3.Text = "Could not connect to scale ";
            }

        }
        void writeToFTP()
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpSite + weightId);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // connect with ftl credentilas
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

            // Copy the contents of the file to the request stream.
            StreamReader sourceStream = new StreamReader(writeTo + "/" + weightId);
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

            response.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                label15.Text = "*";
                label13.Text = "Provide a scale Id";
                return;
            }
            else if (textBox1.Text == "")
            {
                label15.Text = "";
                label8.Text = "*";
                label13.Text = "Give a local address to get the weight";
                return;
            }
            else if (textBox3.Text == "")
            {
                label15.Text = "";
                label8.Text = "";
                label16.Text = "*";
                label13.Text = "Provide a FTP address to upload weight";
                return;
            }
            else if (textBox4.Text == "")
            {
                label15.Text = "";
                label8.Text = "";
                label16.Text = "";
                label17.Text = "*";
                label13.Text = "Provide a FTP username to connect to";
                return;
            }
            else if (textBox5.Text == "")
            {
                label15.Text = "";
                label8.Text = "";
                label16.Text = "";
                label17.Text = "";
                label18.Text = "*";
                label13.Text = "Provide a FTP password to connect to";
                return;
            }
            label18.Text = "";
            label13.Text = "";
            weightId = "weight_" + textBox2.Text + ".json";
            s.Connect();
            System.Windows.Forms.Timer tt = new System.Windows.Forms.Timer();
            tt.Tick += new EventHandler(timer_Tick);     // Everytime timer ticks, timer_Tick will be called
            tt.Interval = 1000;                          // Timer will tick evert second
            tt.Enabled = true;                           // Enable the timer
            button1.Enabled = false;                     // disable Get Weight button
            tt.Start();                                  // Start the timer
       }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
};
