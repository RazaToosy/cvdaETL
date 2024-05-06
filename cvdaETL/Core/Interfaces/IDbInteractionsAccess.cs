using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Models;

namespace cvdaETL.Core.Interfaces
{
    public interface IDbInteractionsAccess
    {
        void InsertInteractions(List<ModelInteraction> Interactions);
        void InsertRecallTeam(ModelRecallTeam RecallTeam);
        Dictionary<string, string> GetCurrentRecallTeam(); 
        bool CheckIfDateExists(DateTime Date);
    }
}
