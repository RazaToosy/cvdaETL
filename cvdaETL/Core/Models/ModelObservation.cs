namespace cvdaETL.Core.Models
{
    public class ModelObservation
    {
        public string ObservationID { get; set; }
        public string PatientID { get; set; }
        public string AppointmentID { get; set; }
        public DateTime ObservationDate { get; set; }
        public string ObservationCodeTerm { get; set; }
        public string ObservationText { get; set; }
        public decimal ObservationValue { get; set; }
        public decimal ObservationValue2 { get; set; }
        public string ObservationType { get; set; }
        public string ObservationTag { get; set; }
        public string ObservationCheckSum { get; set; }

        // Navigation properties
        public ModelPatient Patient { get; set; }
        public ModelAppointment Appointment { get; set; }
    }
}
