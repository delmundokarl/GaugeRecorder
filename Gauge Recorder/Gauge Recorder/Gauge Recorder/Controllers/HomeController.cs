using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gauge_Recorder.Models;
using System.Data.SqlClient;
using Gauge_Recorder.DataHelper;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.IO;

namespace Gauge_Recorder.Controllers
{
    public class HomeController : Controller
    {
        private static string currUser = string.Empty;
        private static List<InputModel> listResult = new List<InputModel>();
        private HomeModels homeModel = new HomeModels();

        public ActionResult Index(LoginModel model)
        {
            ViewBag.Message = "Click the gauge you want to record";

            if (Session["UserName"] != null)
            {
                currUser = (string)Session["Username"];
            }

            homeModel = new HomeModels();

            return View(homeModel);
        }

        [HttpPost]
        public ActionResult Index()
        {
            ViewBag.Message = "Click the gauge you want to record";

            homeModel = new HomeModels();

            return View(homeModel);
        }

        [HttpPost]
        public ActionResult Gauge(int GaugeId, string GaugeName)
        {
            ViewBag.Message = "Specify the value for this gauge.";

            var searchGauge = DataHelper.Connection.RetrieveDataGauge(GaugeId);

            InputModel val = new InputModel();
            val.RelatedGauge = searchGauge.GaugeId;
            val.StandardGauge = searchGauge.MinValue + " - " + searchGauge.MaxValue + " " + searchGauge.Unit;
            val.InputGauge = 0;
            val.Comment = string.Empty;
            val.GaugeType = searchGauge.GaugeName;
            if (Session["UserName"] != null)
            {
                val.Operator = Session["Username"] != null ? (string)Session["Username"] : string.Empty;
            }

            return View(val);
        }

        [HttpGet]
        public ActionResult Gauge(InputModel value)
        {
            if (ModelState.IsValid)
            {
                if (value.InputGauge <= 10)
                {
                    Connection.InsertDataTransaction(value);
                    homeModel = new HomeModels();
                    return View("Index", homeModel);
                }
                else
                {
                    if ((value.Comment != string.Empty) && (value.Comment != null))
                    {
                        Connection.InsertDataTransaction(value);
                        homeModel = new HomeModels();
                        return View("Index", homeModel);
                    }
                    else
                    {
                        value.Comment = "*Required. Specify datails.";
                        return View(value);
                    }
                }
            }

            return View(value);
        }

        [HttpPost]
        public ActionResult DataStorage()
        {
            homeModel = new HomeModels();
            return View(homeModel);
        }

        [HttpPost]
        public ActionResult EditGauge(string comment)
        {
            ViewBag.Comment = comment;

            homeModel = new HomeModels();
            return View(homeModel.Gauge);
        }

        [HttpPost]
        public ActionResult AddGauge()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddGauge(GaugeModel value)
        {
            if (ModelState.IsValid)
            {
                Connection.InsertDataGauge(value);
                homeModel = new HomeModels();
                return View("Index", homeModel);
            }
            else
            {
                return RedirectToAction("EditGauge", "Home", new { comment = "Adding of Gauge Not Successful. Please Try Again." });
            }
        }

        public ActionResult DeleteGauge(int id)
        {
            DataHelper.Connection.DeleteDataGauge(id);
            homeModel = new HomeModels();
            return View("EditGauge", homeModel.Gauge);
        }

        [HttpPost]
        public ActionResult SaveGauge(List<GaugeModel> value)
        {

            if (value != null)
            {
                foreach (var item in value)
                {
                    DataHelper.Connection.UpdateDataGauge(item);
                }
            }
            
            homeModel = new HomeModels();
            return View("EditGauge", homeModel.Gauge);
        }

        //public ActionResult ExportCSV()
        //{
        //    string ResultCsv = ExportFile.CreateCsv(Connection.RetrieveData());

        //    //// Return the file content with response body. 
        //    //Response.ContentType = "text/csv";
        //    //Response.AddHeader("Content-Disposition", "attachment;filename=Faculties.csv");
        //    //Response.Write(ResultCsv);
        //    //Response.End();

        //    var cd = new System.Net.Mime.ContentDisposition
        //    {
        //        // for example foo.bak
        //        FileName = "Output.csv",

        //        // always prompt the user for downloading, set to true if you want 
        //        // the browser to try to show the file inline
        //        Inline = false,
        //    };
        //    Response.AppendHeader("Content-Disposition", cd.ToString());

        //    return File(attachment.FileName, "application/force-download", Path.GetFileName(attachment.FileName)   new System.Text.UTF8Encoding().GetBytes(ResultCsv), "text/csv", "Output.csv");
        //}

        //public HttpResponseMessage ExportCSV()
        //{
        //    string content = "Hello";

        //    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //    result.Content = new StringContent(content);
        //    //a text file is actually an octet-stream (pdf, etc)
        //    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //    //we used attachment to force download
        //    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //    result.Content.Headers.ContentDisposition.FileName = "mytext.txt";
        //    return result;
        //}

        //[HttpPost]
        //public ActionResult ExportClientsListToCSV()
        //{
        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", "attachment;filename=Exported_Users.csv");
        //    Response.ContentType = "text/csv";

        //    string ResultCsv = ExportFile.CreateCsv(Connection.RetrieveDataTransaction());

        //    Response.Write(ResultCsv);

        //    Response.End();

        //    var a = File(new System.Text.UTF8Encoding().GetBytes(ResultCsv), "text/csv", "Report123.csv");

        //    return View(a);
        //}

        [HttpPost]
        public ActionResult ExportClientsListToCSV()
        {
            string content = "Hello, frsfr";

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent(content);
            //a text file is actually an octet-stream (pdf, etc)
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            //we used attachment to force download
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "mytext.txt";

            var a = File(new System.Text.UTF8Encoding().GetBytes(content), "text/csv", "Report123.csv");

            return View(a);
        }

        public FileContentResult DownloadCSV()
        {
            string csv = "Charlie, Chaplin, Chuckles";
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Report123.csv");
        }

        //public FileContentResult DownloadCSV()
        //{

        //    var csvStringData = new StreamReader(Request.InputStream).ReadToEnd();

        //    csvStringData = Uri.UnescapeDataString(csvStringData.Replace("mydata=", ""));

        //    return File(new System.Text.UTF8Encoding().GetBytes(csvStringData), "text/csv", "report.csv");
        //}
    }
}
