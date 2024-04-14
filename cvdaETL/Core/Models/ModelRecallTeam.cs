namespace cvdaETL.Core.Models
{
    public class ModelRecallTeam
    {
        public string RecallTeamID { get; set; }
        public string RecallTeamName { get; set; }

        // Navigation property
        public ICollection<ModelInteraction> Interactions { get; set; }
    }
}
