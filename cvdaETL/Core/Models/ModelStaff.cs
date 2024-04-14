namespace cvdaETL.Core.Models
{
    public class ModelStaff
    {
        public string StaffID { get; set; }
        public string StaffName { get; set; }
        public string StaffDesignation { get; set; }

        // Navigation property
        public ICollection<ModelClinic> Clinics { get; set; }
    }
}
