using System.Collections;

namespace cvdaETL.Core.Models
{
    public class ModelRegister
    {
        public string RegisterID { get; set; }
        public string PatientID { get; set; }
        public DateTime RegisterDate { get; set; }
        public string RegisterState { get; set; }

        // Navigation property
        public ICollection<ModelPatient> Patients { get; set; }
    }
}
