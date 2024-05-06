using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using cvdaETL.Core.Models;

namespace cvdaETL.Data
{
    public class Repo
    {
        private static Repo _instance;
        public static Repo Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("Repo instance has not been initialized.");
                }
                return _instance;
            }
        }

        public string ConnectionString { get; set; }
        public DateTime InsertDate { get; set; }
        public string CsvPath { get; set; }
        public Dictionary<string, string> PatientIDsNHSNumber { get; set; }
        public Dictionary<string, List<string>> CvdaTargetMaps { get; }
        public List<ModelAppointment> CvdaAppointments { get; set; }
        public List<ModelClinic> CvdaClinics { get; set; }
        public List<ModelCondition> CvdaConditions { get; set; }
        public List<ModelInteraction> CvdaInteractions { get; set; }
        public List<ModelObservation> CvdaObservations { get; set; }
        public List<ModelPatient> CvdaPatients { get; set; }
        public List<ModelRegister> CvdaRegisters { get; set; }
        public List<ModelStaff> CvdaStaff { get; set; }
        public List<ModelTarget> CvdaTargets { get; set; }

        private Repo(string Connectionstring, string Csvpath, DateTime DateToInsert)
        {
            InsertDate = DateToInsert;
            ConnectionString = Connectionstring;
            CsvPath = Csvpath;
            PatientIDsNHSNumber= new Dictionary<string, string>();
            CvdaTargetMaps = importCVDATargets(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CVDATargets.xml"));
            CvdaAppointments = new List<ModelAppointment>();
            CvdaClinics = new List<ModelClinic>();
            CvdaConditions = new List<ModelCondition>();
            CvdaInteractions = new List<ModelInteraction>();
            CvdaObservations = new List<ModelObservation>();
            CvdaPatients = new List<ModelPatient>();
            CvdaRegisters = new List<ModelRegister>();
            CvdaStaff = new List<ModelStaff>();
            CvdaTargets = new List<ModelTarget>();
        }

        public static void Initialize(string Connectionstring, string Csvpath, DateTime DateToInsert)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("Repo instance has already been initialized.");
            }
            _instance = new Repo(Connectionstring, Csvpath, DateToInsert);
        }

        private Dictionary<string, List<string>> importCVDATargets(string xmlString)
        {
            var xDocument = XDocument.Parse(File.ReadAllText(xmlString));
            var dictionary = new Dictionary<string, List<string>>();

            foreach (var condition in xDocument.Descendants("Condition"))
            {
                var conditionName = condition.Attribute("name")?.Value;
                var targetsList = new List<string>();

                foreach (var target in condition.Descendants("Target"))
                {
                    // Directly accessing .Value property handles CDATA sections appropriately
                    targetsList.Add(target.Value);
                }

                if (conditionName != null)
                {
                    dictionary[conditionName] = targetsList;
                }
            }
            return dictionary;
        }
    }
}
