using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Data;

namespace cvdaETL.Core.Interfaces
{
    public interface  IDbConditionsAndTargetsAccess
    {
        void InsertConditionsAndTargets(Dictionary<string, string> CVDATargets);
    }
}
