
namespace cvdaETL.Core.Models
{
    public class ModelCondition
    {
        public string ConditionID { get; set; }
        public string PatientID { get; set; } // Foreign key to Patient
        public string ConditionName { get; set; }

        // Navigation properties
        public ModelPatient Patient { get; set; } // One Condition to one Patient
        public ICollection<ModelTarget> Targets { get; set; }
    }
}
