using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TLogger
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                // Log Error
                DataHelper.ErrorLog(string.Format("{0} : {1}", DateTime.Now, ex.Message));
            }            
        }
    }
}
