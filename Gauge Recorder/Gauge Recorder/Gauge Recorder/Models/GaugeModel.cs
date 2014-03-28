using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gauge_Recorder.Models
{
    public class GaugeModel
    {
        public int GaugeId { get; set; }

        [Required]
        public string GaugeName { get; set; }

        [Required]
        public int MinValue { get; set; }

        [Required]
        public int MaxValue { get; set; }

        [Required]
        public string Unit { get; set; }

        public List<InputModel> ValueComparator { get; set; }

        public GaugeModel() { }
    }
}