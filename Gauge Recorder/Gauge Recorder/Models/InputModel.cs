using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gauge_Recorder.Models
{
    public class InputModel
    {
        [Display(Name = "Input Id")]
        public int InputId { get; set; }

        [Display(Name = "Normal Value Range")]
        public string StandardGauge { get; set; }

        [Required]
        [Display(Name = "Input Value")]
        public int InputGauge { get; set; }

        [Display(Name = "Comments")]
        public string Comment { get; set; }

        [Display(Name = "Operator")]
        public string Operator { get; set; }

        [Display(Name = "Gauge Type")]
        public string GaugeType { get; set; }

        [Display(Name = "Related Gauge")]
        public int RelatedGauge { get; set; }
    }
}