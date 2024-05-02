namespace cvdaETL.Core.Models
{
    public class ModelPatient
    {
        public string PatientID { get; set; }
        public string EmisNo { get; set; }
        public string ODSCode { get; set; }
        public string NHSNumber { get; set; }
        public int RiskScore { get; set; }
        public string HouseBound { get; set; }
        public string InHome { get; set; }
        public string PCN { get; set; }
        public string SurgeryName { get; set; }
        public string UsualGP { get; set; }
        public string Surname { get; set; }
        public string FirstNames { get; set; }
        public string Title { get; set; }
        public string Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string HouseNameFlat { get; set; }
        public string Street { get; set; }
        public string Village { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public string HomeTelephone { get; set; }
        public string Mobile { get; set; }
        public string WorkTelephone { get; set; }
        public string Email { get; set; }
        public int DeprivationDecile { get; set; }
        public int HealthDecile { get; set; }
        public string Ethnicity { get; set; }
        public string PHMData { get; set; }
        public string CVDATargets { get; set; }
        public string CurrentState { get; set; }

        // Navigation properties
        public ICollection<ModelObservation> Observations { get; set; }
        public ICollection<ModelRegister> Registers { get; set; }
        public ICollection<ModelInteraction> Interactions { get; set; }
        public ICollection<ModelAppointment> Appointments { get; set; }
        public ICollection<ModelCondition> Conditions { get; set; }
    }
}
