namespace cvdaETL.Core.Models
{
    public class ModelTarget
    {
        public string TargetID { get; set; }
        public string ConditionID { get; set; } // Foreign key to Condition
        public string PatientID { get; set; } // Foreign key to Patient
        public string CVDATarget { get; set; }
        public DateTime InsertDate { get; set; }

        // Navigation property
        public ModelCondition Condition { get; set; }
    }
}
