using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Interfaces;
using cvdaETL.Core.Models;
using cvdaETL.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;

namespace cvdaETL.Services.DataAccess
{
    public class ConditionsAndTargetsAccdbAccess : IDbConditionsAndTargetsAccess
    {
        private string _connectionString;
        public ConditionsAndTargetsAccdbAccess()
        {
            _connectionString = Repo.Instance.ConnectionString;
        }

        public void InsertConditionsAndTargets(Dictionary<string, string> CVDATargets)
        {
            var modelConditons = new List<ModelCondition>();
            var modelTargets = new List<ModelTarget>();

            //dictionary is <patientID, Cvdatargets>
            foreach (KeyValuePair<string,string> kvp in CVDATargets)
            {
                if (kvp.Value == string.Empty) continue;
                
                var patientID = kvp.Key;
                var targets = kvp.Value.Split(",");
                var conditions = new Dictionary<string, string>();
                foreach (var target in targets)
                {
                    var condition = findConditionForTarget(target);
                    if (!conditions.ContainsKey(condition))
                    {
                        conditions.Add(condition,Guid.NewGuid().ToString());
                        var modelCondition = new ModelCondition
                        {
                            ConditionID = conditions[condition],
                            PatientID = patientID,
                            ConditionName = condition,
                            InsertDate = Repo.Instance.InsertDate
                        };
                        InsertCondition(modelCondition);
                        modelConditons.Add(modelCondition);
                    }

                    var modelTarget = new ModelTarget
                    {
                        TargetID = Guid.NewGuid().ToString(),
                        ConditionID = conditions[findConditionForTarget(target)],
                        PatientID = patientID,
                        CVDATarget = target,
                        InsertDate = Repo.Instance.InsertDate
                    };
                    InsertTarget(modelTarget);
                    modelTargets.Add(modelTarget);
                }

            }

            Repo.Instance.CvdaConditions = modelConditons;
            Repo.Instance.CvdaTargets = modelTargets;

        }

        private void InsertCondition(ModelCondition Condition)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                var sql = "INSERT INTO Conditions (ConditionID, PatientID, ConditionName, InsertDate) VALUES (@ConditionID, @PatientID, @ConditionName, @InsertDate)";

                connection.Execute(sql, new
                {
                    ConditionID = Condition.ConditionID,
                    PatientID = Condition.PatientID,
                    ConditionName = Condition.ConditionName,
                    InsertDate = Condition.InsertDate// DateTime.ParseExact(Condition.InsertDate.ToString("MM/dd/yyyy"), "MM/dd/yyyy", CultureInfo.InvariantCulture)
                });

            }
        }

        private void InsertTarget(ModelTarget Target)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                var sql = "INSERT INTO Targets (TargetID, ConditionID, PatientID, CVDATarget, InsertDate) VALUES (@TargetID, @ConditionID, @PatientID, @CVDATarget, @InsertDate)";
                connection.Execute(sql, new
                {
                    TargetID = Target.TargetID,
                    ConditionID = Target.ConditionID,
                    PatientID = Target.PatientID,
                    CVDATarget = Target.CVDATarget,
                    InsertDate = Target.InsertDate// DateTime.ParseExact(Target.InsertDate.ToString("MM/dd/yyyy"), "MM/dd/yyyy", CultureInfo.InvariantCulture)
                });
            }
        }

        private string findConditionForTarget(string target)
        {
            foreach (var condition in Repo.Instance.CvdaTargetMaps)
            {
                if (condition.Value.Contains(target.Trim()))
                {
                    return condition.Key;
                }
            }
            return null;
        }
    }
}
