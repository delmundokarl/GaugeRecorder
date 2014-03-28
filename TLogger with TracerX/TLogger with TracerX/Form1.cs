
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
using TracerX;
using System.Threading;


namespace TLogger
{
    public partial class Form1 : Form
    {
        #region Logger Function

        private static readonly Logger Log = Logger.GetLogger("Program");

        // Initialize TracerX early.
        private static bool LogFileOpened = InitLogging();

        // Initialize the TracerX logging system.
        private static bool InitLogging()
        {
            try
            {
                // Name of threads.
                Thread.CurrentThread.Name = "MainThread";

                // Enable Circular function
                Logger.DefaultBinaryFile.Directory = "C:\\Logs";
                Logger.DefaultTextFile.Directory = "C:\\Logs";

                Logger.DefaultBinaryFile.FullFilePolicy = FullFilePolicy.Wrap;
                Logger.DefaultTextFile.FullFilePolicy = FullFilePolicy.Wrap;

                Logger.DefaultBinaryFile.CircularStartSizeKb = 1;
                Logger.DefaultTextFile.CircularStartSizeKb = 1;

                // Load TracerX configuration from an XML file.
                Logger.Xml.Configure("TracerX.xml");

                // Open the log file.
                Logger.DefaultBinaryFile.Open();
                Logger.DefaultTextFile.Open();

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        const int WordCounts = 5;

        // OPCサーバーアクセスインスタンス
        DxpSimpleAPI.DxpSimpleClass opc = new DxpSimpleAPI.DxpSimpleClass();
        string[] sItemIDArray = new string[WordCounts];

        public Form1()
        {
            InitializeComponent();
        }

        // ロード時サーバーリストを更新する
        private void Form1_Load(object sender, EventArgs e)
        {
            try
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

                using (Log.InfoCall())
                {
                    Log.Info("Program Started");
                }
            }
            catch (Exception ex)
            {
                // Log Error
                //DataHelper.ErrorLog(string.Format("{0} : {1}", DateTime.Now, ex.Message));

                using (Log.InfoCall())
                {
                    Log.Info(string.Format("{0} : {1}", DateTime.Now, ex.Message));
                }

            }
        }

        // 接続
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (opc.Connect(txtNode.Text, cmbServerList.Text))
                {
                    btnListRefresh.Enabled = false;
                    btnDisconnect.Enabled = true;
                    btnConnect.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                // Log Error
                //DataHelper.ErrorLog(string.Format("{0} : {1}", DateTime.Now, ex.Message));

                using (Log.InfoCall())
                {
                    Log.Info(string.Format("{0} : {1}", DateTime.Now, ex.Message));
                }
            }
        }

        // 接続解除
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (opc.Disconnect())
                {
                    btnConnect.Enabled = true;
                    btnListRefresh.Enabled = true;
                    btnDisconnect.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                // Log Error
                //DataHelper.ErrorLog(string.Format("{0} : {1}", DateTime.Now, ex.Message));

                using (Log.InfoCall())
                {
                    Log.Info(string.Format("{0} : {1}", DateTime.Now, ex.Message));
                }
            }
        }

        // ノードに対するサーバーリストのリフレッシュ
        private void btnListRefresh_Click(object sender, EventArgs e)
        {
            cmbServerList.Items.Clear();
            string[] ServerNameArray;

            try
            {
                opc.EnumServerList(txtNode.Text, out ServerNameArray);

                for (int a = 0; a < ServerNameArray.Count<string>(); a++)
                {
                    cmbServerList.Items.Add(ServerNameArray[a]);
                }
                cmbServerList.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // Log Error
                // DataHelper.ErrorLog(string.Format("{0} : {1}", DateTime.Now, ex.Message));

                using (Log.InfoCall())
                {
                    Log.Info(string.Format("{0} : {1}", DateTime.Now, ex.Message));
                }
            }
        }

        /// <summary>
        /// タスクバーの終了メニューイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                this.opc.Disconnect();
                this.Close();
            }
            catch (Exception ex)
            {
                // Log Error
                // DataHelper.ErrorLog(string.Format("{0} : {1}", DateTime.Now, ex.Message));

                using (Log.InfoCall())
                {
                    Log.Info(string.Format("{0} : {1}", DateTime.Now, ex.Message));
                }
            }
        }

        /// <summary>
        /// トリガーレジスターを監視
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] trigger = { Settings.Default.DeviceName + "." + Settings.Default.TriggerAddress, };
            object[] value;
            short[] quality;
            FILETIME[] time;
            int[] error;

            try
            {
                opc.Read(trigger, out value, out quality, out time, out error);

                int i = int.Parse(value[0].ToString());
                //            if ((i & Settings.Default.TriggerOffset) != 0)
                for (int l = 0; l < Settings.Default.LoopTime; l++)
                {
                    if ((i & (0x01 << l)) != 0)
                    {
                        ReadTargetValues(l);
                        //value[0] = i ^ Settings.Default.TriggerOffset;
                        //opc.Write(trigger, value, out error);
                    }
                }

                // Log Error
                for (int j = 0; j < error.Length; j++)
                {
                    //DataHelper.ErrorLog(string.Format("{0} : OPC.ReadError at index={1}, Value={2}", DateTime.Now, j, error[j]));

                    using (Log.InfoCall())
                    {
                        Log.Info(string.Format("{0} : OPC.ReadError at index={1}, Value={2}", DateTime.Now, j, error[j]));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log Error
                // DataHelper.ErrorLog(string.Format("{0} : {1}", DateTime.Now, ex.Message));

                using (Log.InfoCall())
                {
                    Log.Info(string.Format("{0} : {1}", DateTime.Now, ex.Message));
                }
            }
        }

        /// <summary>
        /// 設定されたアドレスの値をCSVファイル吐き出し
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

                int hexNum = int.Parse(Settings.Default.TergetQuantity, System.Globalization.NumberStyles.AllowHexSpecifier);
                string[] sItemIDArray = new string[hexNum];

                for (int num = hexNum * startNum; num < (hexNum * startNum) + hexNum; num++)
                {
                    int temp = int.Parse(Settings.Default.TargetNumber, System.Globalization.NumberStyles.AllowHexSpecifier) + num;
                    sItemIDArray[num - (hexNum * startNum)] = Settings.Default.DeviceName + "." + Settings.Default.TargetPrefix + temp.ToString("X");
                }

                if (opc.Read(sItemIDArray.ToArray(), out oValueArray, out wQualityArray, out fTimeArray, out nErrorArray) == true)
                {
                    string fileName = "";
                    //                    fileName = MakeFileNameFromBCD(oValueArray);
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
                        //                        sb.AppendLine(oValueArray[b * 40 + 39].ToString());    // 行末はカンマなし
                    }
                    File.WriteAllText(fileName, sb.ToString(), Encoding.Unicode);
                }
                else
                {
                    // Log Error
                    // DataHelper.ErrorLog(string.Format("{0} : Error in function ReadTargetValues()", DateTime.Now));

                    using (Log.InfoCall())
                    {
                        Log.Info(string.Format("{0} : Error in function ReadTargetValues()", DateTime.Now));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log Error
                // DataHelper.ErrorLog(string.Format("{0} : {1}", DateTime.Now, ex.Message));

                using (Log.InfoCall())
                {
                    Log.Info(string.Format("{0} : {1}", DateTime.Now, ex.Message));
                }
            }
        }

        /// <summary>
        /// BCD値からファイル名を作成
        /// </summary>
        /// <param name="oValueArray"></param>
        /// <returns></returns>
        private string MakeFileNameFromBCD(object[] oValueArray)
        {
            string fileName = string.Empty;

            try
            {
                fileName = GetBCD(int.Parse(oValueArray[0].ToString()), 4);     // 年
                fileName += GetBCD(int.Parse(oValueArray[1].ToString()), 2);     // 月
                fileName += GetBCD(int.Parse(oValueArray[2].ToString()), 2);     // 日
                fileName += GetBCD(int.Parse(oValueArray[3].ToString()), 4);     // 時分
                fileName += GetBCD(int.Parse(oValueArray[4].ToString()), 2);     // 秒
            }
            catch (Exception ex)
            {
                // Log Error
                // DataHelper.ErrorLog(string.Format("{0} : {1}", DateTime.Now, ex.Message));

                using (Log.InfoCall())
                {
                    Log.Info(string.Format("{0} : {1}", DateTime.Now, ex.Message));
                }
            }

            return fileName;
        }

        /// <summary>
        /// BCD値を文字列に変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private string GetBCD(int value, int num)
        {
            string result = "";

            try
            {
                for (int b = (num - 1) * 4; b >= 0; b -= 4)
                {
                    result += ((value >> b) & 0x0f).ToString();
                }
            }
            catch (Exception ex)
            {
                // Log Error
                // DataHelper.ErrorLog(string.Format("{0} : {1}", DateTime.Now, ex.Message));

                using (Log.InfoCall())
                {
                    Log.Info(string.Format("{0} : {1}", DateTime.Now, ex.Message));
                }
            }

            return result;
        }

        /// <summary>
        /// フォーム表示時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
