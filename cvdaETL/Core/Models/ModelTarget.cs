namespace cvdaETL.Core.Models
{
    public class ModelTarget
    {
        public string TargetID { get; set; }
        public string ConditionID { get; set; } // Foreign key to Condition
        public string CVDATarget { get; set; }

        // Navigation property
        public ModelCondition Condition { get; set; }
    }
}
