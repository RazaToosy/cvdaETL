namespace cvdaETL.Core.Models
{
    public class ModelClinic
    {
        public string ClinicID { get; set; }
        public string ClinicName { get; set; }
        public string StaffID { get; set; }
        public string ClinicLocation { get; set; }
        public string ClinicPostCode { get; set; }
        public string ClinicType { get; set; }
        public string ClinicDate { get; set; }
        public int ClinicDuration { get; set; }

        // Navigation properties
        public ICollection<ModelAppointment> Appointments { get; set; }
        public ModelStaff Staff { get; set; }
    }
}

