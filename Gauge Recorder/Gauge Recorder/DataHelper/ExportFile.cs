using Gauge_Recorder.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Gauge_Recorder.DataHelper
{
    public class ExportFile
    {
        public static string CreateCsv(List<InputModel> input)
        {

            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\output.csv";
            string delimiter = ",";

            int length = input.Count;

            StringBuilder sb = new StringBuilder();

            // Header
            sb.AppendLine("Gauge Type,Standard Gauge,Input Gauge,Operator,Comment");

            try
            {
                foreach (var item in input)
                {
                    sb.Append(item.GaugeType + delimiter);
                    sb.Append(item.StandardGauge.ToString() + delimiter);
                    sb.Append(item.InputGauge.ToString() + delimiter);
                    sb.Append(item.Operator + delimiter);
                    sb.Append(item.Comment + delimiter);

                    sb.AppendLine();
                }
            }
            catch (Exception ex)
            {
                // Error
            }

            return sb.ToString();
        }
    }

}