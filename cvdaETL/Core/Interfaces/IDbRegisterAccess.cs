using cvdaETL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdaETL.Core.Interfaces
{
    public interface IDbRegisterAccess
    {
        void InsertRegister(List<ModelRegister> register);
    }
}
