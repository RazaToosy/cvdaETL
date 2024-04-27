using cvdaETL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdaETL.Core.Interfaces
{
    public interface IDbPatientAccess
    {
        void InsertPatients(IEnumerable<ModelPatient> patients);
        void UpdatePatients(IEnumerable<ModelPatient> patients);
        Dictionary<string, string> GetNHSNumbers();
    }
}
