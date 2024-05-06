using cvdaETL.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Models;
using cvdaETL.Core.Maps;
using cvdaETL.Data;
using cvdaETL.Services.CsvHelper;
using cvdaETL.Core.Enums;

namespace cvdaETL.Services.ETLManager
{
    public class InteractionsProcessor
    {
        ResolveDb _dbAccess;
        private Dictionary<string, string> _recallTeam;

        public InteractionsProcessor(ResolveDb dbAccess)
        {
            _dbAccess = dbAccess;
            _recallTeam = new Dictionary<string, string>();
        }

        public void ProcessInteractions()
        {
            //Import into from Csv
            var interactions = new CsvHelperManager().ImportFromCsv<ModelInteraction, InteractionsMap>(Path.Combine(Repo.Instance.CsvPath, "Interactions.csv"));

            _recallTeam = _dbAccess.InteractionsAccess.GetCurrentRecallTeam();

            // Extract RecallTeam names from imported interactions and find names not in existingRecallTeamNames
            var recallTeamNamesNotInExisting = interactions
                .Select(i => i.RecallTeamName)
                .Distinct()
                .Where(name => !_recallTeam.ContainsValue(name))
                .ToList();

            recallTeamNamesNotInExisting.ForEach(name =>
            {
                var teamMember = new ModelRecallTeam
                {
                    RecallTeamID = Guid.NewGuid().ToString(),
                    RecallTeamName = name
                };
                // Insert the RecallTeam into the database
                _dbAccess.InteractionsAccess.InsertRecallTeam(teamMember);
                _recallTeam.Add(teamMember.RecallTeamID, teamMember.RecallTeamName);
            });

            // Remove interactions where the NHSnumber is empty
            interactions.RemoveAll(i => string.IsNullOrEmpty(i.NHSNumber));

            interactions = getMatchingInteractions(interactions); //get interactions with 3 entries on the same date and one with "Healthy Heart H/C SPOC" in the comments

            interactions = createTypeandMakeDistinct(interactions); //create interaction type and make distinct
            
            _dbAccess.InteractionsAccess.InsertInteractions(interactions);

        }
        private List<ModelInteraction> getMatchingInteractions(List<ModelInteraction> Interactions)
        {
            // Group interactions by date
            var groupedInteractions = Interactions.GroupBy(i => new { i.InteractionDate, i.NHSNumber }).ToList();

            // Filter the grouped interactions to include only those with three entries on the same date
            var matchingGroups = groupedInteractions.Where(g => g.Count() == 3).ToList();

            // Find the groups that have at least one entry with "Healthy Heart H/C SPOC" in the Comments field
            var matchingInteractions = matchingGroups
                .Where(g => g.Any(i => i.InteractionComments.Contains("Healthy Heart H/C SPOC")))
                .SelectMany(g => g)
                .ToList();

            return matchingInteractions;
        }

        private List<ModelInteraction> createTypeandMakeDistinct(List<ModelInteraction> Interactions)
        {
            // Group interactions by date
            var groupedInteractions = Interactions.GroupBy(i => new { i.InteractionDate, i.NHSNumber }).ToList();
            List<ModelInteraction> matchingInteractions = new List<ModelInteraction>();

            // Text patterns and their corresponding InteractionTypes
            var interactionTypeMappings = new Dictionary<string, InteractionType>
                {
                    { "Appointment made by telephone", InteractionType.Booked },
                    { "Invited but not booked", InteractionType.InvitedButNotBooked },
                    { "Failed encounter", InteractionType.FailedEncounter },
                    { "Cardiovascular disease high risk review declined", InteractionType.Declined}
                };

            // Function to map text patterns to an InteractionType
            InteractionType GetInteractionType(string codeTerm, string comments)
            {
                foreach (var kvp in interactionTypeMappings)
                {
                    if (codeTerm.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase) ||
                        comments.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        return kvp.Value;
                    }
                }

                return InteractionType.Unknown;
            }

            foreach (var group in groupedInteractions)
            {
                if (_dbAccess.InteractionsAccess.CheckIfDateExists(group.Key.InteractionDate)) continue; //If date

                // Find the first interaction that matches any of the specific text patterns
                var entryToKeep = group.FirstOrDefault(i =>
                    interactionTypeMappings.Keys.Any(key =>
                        i.InteractionCodeTerm.Contains(key, StringComparison.OrdinalIgnoreCase) ||
                        i.InteractionComments.Contains(key, StringComparison.OrdinalIgnoreCase)));

                if (entryToKeep != null)
                {
                    var patientID = Repo.Instance.PatientIDsNHSNumber.FirstOrDefault(x => x.Value == entryToKeep.NHSNumber).Key;

                    if (patientID == null) continue;
                    // Determine the interaction type based on the specific text pattern
                    var newType = GetInteractionType(entryToKeep.InteractionCodeTerm, entryToKeep.InteractionComments);

                    // Create a new instance or update the existing one
                    var newInteraction = new ModelInteraction
                    {
                        InteractionID = Guid.NewGuid().ToString(),
                        PatientID = patientID,
                        NHSNumber = entryToKeep.NHSNumber,
                        RecallTeamID = _recallTeam.FirstOrDefault(r=>r.Value == entryToKeep.RecallTeamName).Key,
                        RecallTeamName = entryToKeep.RecallTeamName,
                        InteractionDate = entryToKeep.InteractionDate,
                        InteractionCodeTerm = entryToKeep.InteractionCodeTerm,
                        InteractionComments = entryToKeep.InteractionComments,
                        InteractionType = newType.ToString(),
                    };

                    matchingInteractions.Add(newInteraction);
                }
            }

            return matchingInteractions;

        }

    }
}
