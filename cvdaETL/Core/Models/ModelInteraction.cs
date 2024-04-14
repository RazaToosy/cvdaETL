namespace cvdaETL.Core.Models
{
    public class ModelInteraction
    {
        public string InteractionID { get; set; }
        public string PatientID { get; set; }
        public string RecallTeamID { get; set; }
        public DateTime InteractionDate { get; set; }
        public string InteractionCodeTerm { get; set; }
        public string InteractionExclusionText { get; set; }
        public string InteractionConceptID { get; set; }
        public string InteractionComments { get; set; }

        // Navigation properties
        public ICollection<ModelAppointment> Appointments { get; set; }
    }
}

