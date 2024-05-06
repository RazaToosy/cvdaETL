
namespace cvdaETL.Core.Models
{
    public class ModelAppointment
    {
        public string AppointmentID { get; set; }
        public string InteractionID { get; set; }
        public string StaffID { get; set; }
        public string PatientID { get; set; }
        public string NHSNumber { get; set; }
        public string StaffName { get; set; }
        public string StaffRole { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string AppointmentTime { get; set; }
        public int AppointmentPlannedTime { get; set; }
        public int AppointmentActualTime { get; set; }
        public string AppointmentLocation { get; set; }
        public string AppointmentPostCode { get; set; }
        public string AppointmentType { get; set; }
        public string AppointmentMode { get; set; }
        public string AppointmentOutcome { get; set; }
        public string AppointmentFrequency { get; set; }


        // Navigation properties
        public ModelPatient Patient { get; set; }
        public ModelClinic Clinic { get; set; }
        public ModelInteraction IInteraction { get; set; }
        public ICollection<ModelObservation> Observation { get; set; }
    }
}
