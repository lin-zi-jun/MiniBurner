using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MiniBurner
{
    public partial class Form1 : Form
    {
        string toolpath = Application.StartupPath + "\\esptool.exe";
        string binpath = "";

        public Form1()
        {
            InitializeComponent();

            this.Text = "EasyLoader";
            label4.Text = "APP_BALA";
            this.Icon = Resource1.m5;
            GetSerial();
            if(comboBox1.Items.Count > 0)
            {
                comboBox1.Text = comboBox1.Items[0].ToString();
            }
            comboBox2.Text = comboBox2.Items[0].ToString();

            binpath = Application.StartupPath + "\\firmware_" + label4.Text + ".bin";
        }

        private void GetSerial()
        {
            string[] ports = SerialPort.GetPortNames();
            ports.ToList().ForEach(p =>
            {
                comboBox1.Items.Add(p);
            });
        }

        private bool ValidateForm()
        {
            if(comboBox1.Text == "")
            {
                MessageBox.Show("COM is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if(comboBox2.Text == "")
            {
                MessageBox.Show("Buadrate is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void SaveToopAsTemp()
        {
            if (File.Exists(toolpath)) return;
            string esptool = toolpath;
            byte[] b = Resource1.esptool;
            MemoryStream ms = new MemoryStream(b);
            FileStream fs = new FileStream(esptool, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(b);
            bw.Close();
            fs.Close();
            File.SetAttributes(esptool, FileAttributes.Hidden);
        }

        private void SaveBinAsTemp()
        {
            if (File.Exists(binpath)) return;
            string bin = binpath;
            byte[] b = Resource1.firmware;
            MemoryStream ms = new MemoryStream(b);
            FileStream fs = new FileStream(bin, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(b);
            bw.Close();
            fs.Close();
            File.SetAttributes(bin, FileAttributes.Hidden);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;
            SaveToopAsTemp();
            SaveBinAsTemp();
            string esptool = toolpath;
            Process p = new Process();
            p.StartInfo.FileName = esptool;
            p.StartInfo.Arguments = String.Format("--chip esp32 --port {0} --baud {1} write_flash -z --flash_mode dio --flash_freq 40m --flash_size detect 0x1000 \"{2}\"", comboBox1.Text, comboBox2.Text, binpath);
            p.EnableRaisingEvents = true;
            p.Exited += P_Exited;
            p.Start();
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void P_Exited(object sender, EventArgs e)
        {
            BeginInvoke(new Action(delegate
            {
                int status = ((Process)sender).ExitCode;
                if (status == -1)
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                    label3.ForeColor = Color.Red;
                    label3.Text = "Failed.";
                }
                else if (status == 0)
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                    label3.ForeColor = Color.Green;
                    label3.Text = "Successfully.";
                }
                else
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                    label3.ForeColor = Color.Red;
                    label3.Text = "Failed.";
                }
                string esptool = toolpath;
                string bin = binpath;
                if (File.Exists(esptool))
                {
                    try
                    {
                        File.Delete(esptool);
                    }
                    catch { }
                }
                if (File.Exists(bin))
                {
                    try
                    {
                        File.Delete(bin);
                    }
                    catch { }
                }
            }));
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            GetSerial();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;
            SaveToopAsTemp();
            string esptool = toolpath;
            Process p = new Process();
            p.StartInfo.FileName = esptool;
            p.StartInfo.Arguments = String.Format("--chip esp32 --port {0} erase_flash", comboBox1.Text);
            p.EnableRaisingEvents = true;
            p.Exited += P_Exited;
            p.Start();
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            string esptool = toolpath;
            string bin = binpath;
            if (File.Exists(esptool))
            {
                try
                {
                    File.Delete(esptool);
                }
                catch { }
            }
            if (File.Exists(bin))
            {
                try
                {
                    File.Delete(bin);
                }
                catch { }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Label4_Click(object sender, EventArgs e)
        {

        }
    }
}
