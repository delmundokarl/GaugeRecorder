using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TLogger
{
    public class DataHelper
    {
        public static void ErrorLog(string error)
        {
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(Path.GetFullPath("../ErrorLogs.txt"), true);

                //Write a line of text
                sw.WriteLine(error);

                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                // Error
            }
        }
    }
}
