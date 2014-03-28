using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Gauge_Recorder.Models
{
    public class HomeModels
    {
        private List<GaugeModel> _gauge;
        public List<GaugeModel> Gauge { get { return _gauge; } }

        private List<InputModel> _inputList;
        public List<InputModel> InputList { get { return _inputList; } }

        public HomeModels() {
            // Initialize
            _gauge = new List<GaugeModel>();
            _inputList = new List<InputModel>();

            // Populate
            _gauge = DataHelper.Connection.RetrieveDataGauge();
            _inputList = DataHelper.Connection.RetrieveDataTransaction();
        }
    }
}