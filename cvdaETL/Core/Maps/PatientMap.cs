using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Models;

namespace cvdaETL.Core.Maps
{
    public sealed class PatientMap : ClassMap<ModelPatient>
    {
        public PatientMap()
        {
            Map(m => m.PatientID).Name("PatientID");
            Map(m => m.EmisNo).Name("EmisNo");
            Map(m => m.ODSCode).Name("ODSCode");
            Map(m => m.NHSNumber).Name("NHSNumber"); 
            Map(m => m.RiskScore).Name("AllConditionsRSTPHMCombined");
            Map(m => m.HouseBound).Name("HOUSEBOUND");
            Map(m => m.InHome).Name("INHOME");
            Map(m => m.PCN).Name("PCN");
            Map(m => m.SurgeryName).Name("SurgeryName");
            Map(m => m.UsualGP).Name("UsualGP");
            Map(m => m.Surname).Name("SurName");
            Map(m => m.FirstNames).Name("FirstNames");
            Map(m => m.Title).Name("Title");
            Map(m => m.Sex).Name("Sex");
            Map(m => m.DateOfBirth).Name("DateOfBirth");
            Map(m => m.Age).Name("Age");
            Map(m => m.HouseNameFlat).Name("HouseNameFlat");
            Map(m => m.Street).Name("Street");
            Map(m => m.Village).Name("Village");
            Map(m => m.Town).Name("Town");
            Map(m => m.County).Name("County");
            Map(m => m.PostCode).Name("PostCode");
            Map(m => m.HomeTelephone).Name("HomeTelephone");
            Map(m => m.Mobile).Name("Mobile");
            Map(m => m.WorkTelephone).Name("WorkTelephone");
            Map(m => m.Email).Name("Email");
            Map(m => m.DeprivationDecile).Name("DeprivationDecile");
            Map(m => m.HealthDecile).Name("HealthDecile");
            Map(m => m.Ethnicity).Name("Ethnicity");
            Map(m => m.PHMData).Name("PHMData");
        }
    }
}
