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

namespace GetWeightUSBScale
{
    public partial class Form1 : Form
    {
        decimal? weight;
        bool? isStable;
        string writeTo = "C:/Users/Machool/AppData/uDoApp";

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

            s.Connect();
            textBox1.Text = writeTo;
            System.Windows.Forms.Timer tt = new System.Windows.Forms.Timer();
            tt.Tick += new EventHandler(timer_Tick);     // Everytime timer ticks, timer_Tick will be called
            tt.Interval = 1000;                          // Timer will tick evert second
            tt.Enabled = true;                           // Enable the timer
            tt.Start();                                  // Start the timer
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (s.IsConnected)
            {

                s.GetWeight(out weight, out isStable);
                //s.DebugScaleData();
                s.Disconnect();
                string unit = "Kgs";
                if (Convert.ToInt16(s.ScaleWeightUnits) == 12) unit = "Lbs";
                //Console.WriteLine("Weight: {0:0.00} " + unit, weight);
                string json = "{\"weight\":" + "\"" + weight + "\", \"unit\":\"" + unit + "\"}";
                System.IO.File.WriteAllText(writeTo + @"/weight.json", json);
                label2.Text = "logging " + json;
                label3.Text = "Scale is connected";
                Thread.Sleep(1000);
            }

        }
    }
};
