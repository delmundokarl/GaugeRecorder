
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using OpcRcw.Da;
using System.IO;


namespace TLogger
{
	public partial class Form1 : Form
	{
		DxpSimpleAPI.DxpSimpleClass opc = new DxpSimpleAPI.DxpSimpleClass();

		public Form1()
		{
			InitializeComponent();
		}

		// refresh server list at the loading
		private void Form1_Load(object sender, EventArgs e)
		{
			btnConnect.Enabled = true;
			btnDisconnect.Enabled = false;

            if (opc.Connect(txtNode.Text, cmbServerList.Text))
            {
                btnListRefresh.Enabled = false;
                btnDisconnect.Enabled = true;
                btnConnect.Enabled = false;
            }
            timer1.Enabled = true;
        }

		// connect
		private void btnConnect_Click(object sender, EventArgs e)
		{
			if (opc.Connect(txtNode.Text, cmbServerList.Text))
			{
				btnListRefresh.Enabled = false;
				btnDisconnect.Enabled = true;
				btnConnect.Enabled = false;
			}
		}

		// disconnect
		private void btnDisconnect_Click(object sender, EventArgs e)
		{
			if (opc.Disconnect())
			{
				btnConnect.Enabled = true;
				btnListRefresh.Enabled = true;
				btnDisconnect.Enabled = false;
			}
		}

		// server list refresh at a node
		private void btnListRefresh_Click(object sender, EventArgs e)
		{
			cmbServerList.Items.Clear();
			string[] ServerNameArray;
			opc.EnumServerList(txtNode.Text, out ServerNameArray);

			for (int a = 0; a < ServerNameArray.Count<string>(); a++)
			{
				cmbServerList.Items.Add(ServerNameArray[a]);
			}
			cmbServerList.SelectedIndex = 0;
		}

        /// <summary>
        /// exit event in taskbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.opc.Disconnect();
            this.Close();
        }

        /// <summary>
        /// monitor trigger register
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] trigger = { Settings.Default.DeviceName+"."+Settings.Default.TriggerAddress, };
            object[] value;
            short[]  quality;
            FILETIME[] time;
            int[] error;
            opc.Read(trigger, out value, out quality, out time, out error);

            // when trigger becomes ON, corresponding address values will be output as CSV
            int i = int.Parse(value[0].ToString());
            for (int l = 0; l < Settings.Default.LoopTime; l++)
            {
                if ((i & (0x01<<l)) != 0)
                {
                    ReadTargetValues(l);
                }
            }
        }

        /// <summary>
        /// read values and output CSV
        /// </summary>
        /// <param name="startNum"></param>
        private void ReadTargetValues(int startNum)
        {
            try
            {
                object[] oValueArray;
                short[] wQualityArray;
                FILETIME[] fTimeArray;
                int[] nErrorArray;

                // convert to num value
                int countNum = int.Parse(Settings.Default.TergetQuantity, 
                        System.Globalization.NumberStyles.AllowHexSpecifier);

                 string[] sItemIDArray = null;
                if (Settings.Default.hexCounting)
                {
                    sItemIDArray = MakeHexItem(startNum, countNum);
                }
                else
                {
                    sItemIDArray = MakeDecItem(startNum, countNum);
                }

                if (opc.Read(sItemIDArray.ToArray(), out oValueArray, out wQualityArray, out fTimeArray, out nErrorArray) == true)
                {
                    // making file name. value[0]=year, value[1]=month, value[2]=day, value[3]=hour&min, value[4]=sec
                    string fileName="";
                    fileName = string.Format("{0:0000}{1:00}{2:00}{3:0000}{4:00}", oValueArray[0],
                            oValueArray[1], oValueArray[2], oValueArray[3], 
                            oValueArray[4]);
                    fileName += ".csv";

                    StringBuilder sb = new StringBuilder();
                    for (int a = 0; a < 80; a++)
                    {
                        sb.AppendLine(oValueArray[a].ToString());
                    }
                    for (int b = 0; b < 200; b++)
                    {
                        for (int c = 0; c < 40; c++)
                        {
                            sb.Append(oValueArray[b * 40 + c] + ",");
                        }
                        sb.AppendLine();
                    }
                    File.WriteAllText(fileName, sb.ToString(), Encoding.Unicode);
                }
            }
            catch (Exception ex)
            {
//                MessageBox.Show(ex.ToString());
            }
        }

        private static string[] MakeHexItem(int startNum, int hexNum)
        {
            // output values with hex value range
            string[] sItemIDArray = new string[hexNum];
            for (int num = hexNum * startNum; num < (hexNum * startNum) + hexNum; num++)
            {
                int temp = int.Parse(Settings.Default.TargetNumber, System.Globalization.NumberStyles.AllowHexSpecifier) + num;
                sItemIDArray[num - (hexNum * startNum)] = Settings.Default.DeviceName + "." + Settings.Default.TargetPrefix + temp.ToString("X");
            }
            return sItemIDArray;
        }

        private static string[] MakeDecItem(int startNum, int decNum)
        {
            // output values with dec value range
            string[] sItemIDArray = new string[decNum];
            for (int num = decNum * startNum; num < (decNum * startNum) + decNum; num++)
            {
                int temp = int.Parse(Settings.Default.TargetNumber) + num;
                sItemIDArray[num - (decNum * startNum)] = Settings.Default.DeviceName + "." + Settings.Default.TargetPrefix + temp;
            }
            return sItemIDArray;
        }

        /// <summary>
        /// the form is hide all time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
