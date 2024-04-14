
namespace cvdaETL.Core.Models
{
    public class ModelAppointment
    {
        public string AppointmentID { get; set; }
        public string InteractionID { get; set; }
        public string ClinicID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int AppointmentLength { get; set; }
        public string AppointmentType { get; set; }
        public string AppointmentTerm { get; set; }
        public string AppointmentMode { get; set; }

        // Navigation properties
        public ModelPatient Patient { get; set; }
        public ModelClinic Clinic { get; set; }
        public ModelInteraction IInteraction { get; set; }
        public ICollection<ModelObservation> Observation { get; set; }
    }
}
