using cvdaETL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdaETL.Core.Interfaces
{
    public interface IDbObservationsAccess
    {
        void InsertObservations(List<ModelObservation> Observations);
        List<string> GetCheckSums();
    }
}
