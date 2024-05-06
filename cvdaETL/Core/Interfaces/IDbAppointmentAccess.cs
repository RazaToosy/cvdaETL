using cvdaETL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdaETL.Core.Interfaces
{
    public interface IDbAppointmentAccess
    {
        void InsertAppointments(List<ModelAppointment> Appointments);
        List<ModelAppointment> GetAllAppointments();
        void InsertStaff(ModelStaff Staff);
        Dictionary<string, string> GetStaffWithIDs();
        List<DateTime> GetDatesOfAppointments();
        List<ModelStaff> GetStaffWithRoles();
    }
}
