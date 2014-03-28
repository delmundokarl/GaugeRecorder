using Gauge_Recorder.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Gauge_Recorder.DataHelper
{
    public class Connection
    {
        private static SqlConnection myconnection = new SqlConnection();
        private const string sqlConnection = "Data Source=tcp:yo8zgtjtp5.database.windows.net,1433;Initial Catalog=gaugerecorder_db;User ID=gaugerecorder_db@yo8zgtjtp5;Password=p@ss1word";

        #region Insert Data
        
        public static void InsertDataTransaction(InputModel value)
        {
            myconnection = new SqlConnection(sqlConnection);

            try
            {
                myconnection.Open();

                if (myconnection.State == System.Data.ConnectionState.Open)
                {
                    string query = "INSERT INTO InputTransaction (StandardValue, InputValue, Comment, Operator, GaugeType, RelatedGauge) VALUES ('" + value.StandardGauge + "', '" + value.InputGauge + "', '" + value.Comment + "', '" + value.Operator + "', '" + value.GaugeType + "', '" + value.RelatedGauge + "')";

                    // Create Command
                    SqlCommand cmd = new SqlCommand(query, myconnection);
                    cmd.ExecuteNonQuery();

                    // Close Connection
                    myconnection.Close();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void InsertDataGauge(GaugeModel value)
        {
            myconnection = new SqlConnection(sqlConnection);

            try
            {
                myconnection.Open();

                if (myconnection.State == System.Data.ConnectionState.Open)
                {
                    string query = "INSERT INTO Gauge (GaugeName, MinValue, MaxValue, Unit) VALUES ('" + value.GaugeName + "', '" + value.MinValue + "', '" + value.MaxValue + "', '" + value.Unit + "')";

                    // Create Command
                    SqlCommand cmd = new SqlCommand(query, myconnection);
                    cmd.ExecuteNonQuery();

                    // Close Connection
                    myconnection.Close();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Retrieve Data

        public static List<InputModel> RetrieveDataTransaction()
        {
            List<InputModel> DataList = new List<InputModel>();

            myconnection = new SqlConnection(sqlConnection);

            try
            {
                myconnection.Open();

                if (myconnection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT TransactionId, StandardValue, InputValue, Comment, Operator, GaugeType, RelatedGauge FROM InputTransaction";

                    // Create Command
                    SqlCommand cmd = new SqlCommand(query, myconnection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            InputModel item = new InputModel();
                            item.InputId = CheckIntIfNull(reader.GetInt32(0));
                            item.StandardGauge = CheckStringIfNull(reader.GetString(1));
                            item.InputGauge = CheckIntIfNull(reader.GetInt32(2));
                            item.Comment = CheckStringIfNull(reader.GetString(3));
                            item.Operator = CheckStringIfNull(reader.GetString(4));
                            item.GaugeType = CheckStringIfNull(reader.GetString(5));
                            item.RelatedGauge = CheckIntIfNull(reader.GetInt32(6));
                            DataList.Add(item);
                        }
                    }

                    // Close Connection
                    myconnection.Close();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return DataList;
        }

        public static List<GaugeModel> RetrieveDataGauge()
        {
            List<GaugeModel> DataList = new List<GaugeModel>();

            myconnection = new SqlConnection(sqlConnection);

            try
            {
                myconnection.Open();

                if (myconnection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT GaugeId, GaugeName, MaxValue, MinValue, Unit FROM Gauge";

                    // Create Command
                    SqlCommand cmd = new SqlCommand(query, myconnection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            GaugeModel item = new GaugeModel();
                            item.GaugeId = CheckIntIfNull(reader.GetInt32(0));
                            item.GaugeName = CheckStringIfNull(reader.GetString(1));
                            item.MaxValue = CheckIntIfNull(reader.GetInt32(2));
                            item.MinValue = CheckIntIfNull(reader.GetInt32(3));
                            item.Unit = CheckStringIfNull(reader.GetString(4));
                            DataList.Add(item);
                        }
                    }

                    // Close Connection
                    myconnection.Close();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return DataList;
        }

        public static GaugeModel RetrieveDataGauge(int GaugeId)
        {
            GaugeModel DataList = new GaugeModel();

            myconnection = new SqlConnection(sqlConnection);

            try
            {
                myconnection.Open();

                if (myconnection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT GaugeId, GaugeName, MaxValue, MinValue, Unit FROM Gauge WHERE GaugeId = " + GaugeId.ToString();

                    // Create Command
                    SqlCommand cmd = new SqlCommand(query, myconnection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            GaugeModel item = new GaugeModel();
                            item.GaugeId = CheckIntIfNull(reader.GetInt32(0));
                            item.GaugeName = CheckStringIfNull(reader.GetString(1));
                            item.MaxValue = CheckIntIfNull(reader.GetInt32(2));
                            item.MinValue = CheckIntIfNull(reader.GetInt32(3));
                            item.Unit = CheckStringIfNull(reader.GetString(4));
                            DataList = item;
                        }
                    }

                    // Close Connection
                    myconnection.Close();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return DataList;
        }

        #endregion

        #region Delete Data

        public static void DeleteDataGauge(int value)
        {
            myconnection = new SqlConnection(sqlConnection);

            try
            {
                myconnection.Open();

                if (myconnection.State == System.Data.ConnectionState.Open)
                {
                    string query = "DELETE FROM InputTransaction WHERE RelatedGauge = " + value.ToString();

                    SqlCommand cmd = new SqlCommand(query, myconnection);
                    cmd.ExecuteNonQuery();

                    query = "DELETE FROM Gauge WHERE GaugeId = " + value.ToString();

                    cmd = new SqlCommand(query, myconnection);
                    cmd.ExecuteNonQuery();

                    // Close Connection
                    myconnection.Close();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Update Data

        public static void UpdateDataGauge(GaugeModel value)
        {
            myconnection = new SqlConnection(sqlConnection);

            try
            {
                myconnection.Open();

                if (myconnection.State == System.Data.ConnectionState.Open)
                {
                    string query = "UPDATE Gauge SET GaugeName = '" + value.GaugeName + "', MaxValue = " + value.MaxValue + ", MinValue = " + value.MinValue + ", Unit = '" + value.Unit + "' WHERE GaugeId = " + value.GaugeId;

                    // Create Command
                    SqlCommand cmd = new SqlCommand(query, myconnection);
                    cmd.ExecuteNonQuery();

                    // Close Connection
                    myconnection.Close();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region DataHelper

        public static int CheckIntIfNull(int? val)
        {
            if (!val.HasValue)
            {
                return 0;
            }
            else
            {
                return val.Value;
            }
        }

        public static Guid CheckGuidIfNull(Guid? val)
        {
            if (!val.HasValue)
            {
                return Guid.Empty;
            }
            else
            {
                return val.Value;
            }
        }

        public static string CheckStringIfNull(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                return val;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}