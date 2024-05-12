using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdaETL.Core.Models
{
    public class ModelObservationInBase
    {
        public string NHSNumber { get; set; }
        public decimal Hba1C { get; set; }
        public DateTime? Hba1cDate { get; set; }
        public decimal SystolicBP { get; set; }
        public DateTime? SystolicBPDate { get; set; }
        public decimal DiastolicBP { get; set; }
        public DateTime? DiastolicBPDate { get; set; }
        public decimal Cholesterol { get; set; }
        public DateTime? CholesterolDate { get; set; }
    }
}
