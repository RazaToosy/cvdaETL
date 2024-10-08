﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using cvdaETL.Core.Enums;
using cvdaETL.Core.Models;
using cvdaETL.Data;

namespace cvdaETL.Services.CsvHelper
{
    public class CsvImportObservations
    {
        private List<string> _checksums;

        public CsvImportObservations(List<string> CheckSums)
        {
            _checksums = CheckSums;
        }
        
        public List<ModelObservation> Import(string Filename)
        {
            var observations = new List<ModelObservation>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                IgnoreBlankLines = true,
                HasHeaderRecord = true
            };

            using (var reader = new StreamReader(Filename))
            using (var csv = new CsvReader(reader, config))
            {
                // Read the header row
                csv.Read();
                csv.ReadHeader();
                string[] header = csv.HeaderRecord;

                // Manually read through each row
                while (csv.Read())
                {

                    // Loop through each column by index
                    for (int i = 0; i < header.Length; i++)
                    {
                        string columnName = header[i];
                        string fieldValue = csv.GetField(i);
                        var appointmentID = string.Empty;

                        var dateIndex = 0;

                        if (csv.GetField(5) == string.Empty)
                        {
                            if (csv.GetField(8) != string.Empty) dateIndex = 8;
                        }
                        else dateIndex = 5;

                        if (dateIndex > 0)
                        {
                             DateTime? dateOfClinic = csv.GetField<DateTime>(dateIndex);
                            if (dateOfClinic != null)
                                appointmentID = Repo.Instance.CvdaAppointments
                                    .Where(appt => appt.AppointmentDateTime.Date == dateOfClinic.Value.Date)
                                    .Select(appt => appt.AppointmentID)
                                    .FirstOrDefault() ??string.Empty;                          
                        }
                        else appointmentID = string.Empty;

                        switch (i)
                        {
                            case 5: //Cardiovascular Clinic
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                var observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(5),
                                    ObservationText =  $"{csv.GetField<string>(6)} with {csv.GetField<string>(7)}",
                                    ObservationTag = ObservationTag.CardiovascularClinic.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 8: //Triage
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                 observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                     ObservationDate = csv.GetField<DateTime>(8),
                                    ObservationCodeTerm = csv.GetField<string>(9),
                                    ObservationText = csv.GetField<string>(10),
                                    ObservationTag = ObservationTag.Triage.ToString(),
                                    AppointmentID = appointmentID,
                                     PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 11: //HCA
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(11),
                                    ObservationText = $"{csv.GetField<string>(12)} with {csv.GetField<string>(13)}",
                                    ObservationTag = ObservationTag.HCA.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 14: //Blood Taken
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(14),
                                    ObservationCodeTerm = "Blood Sample Taken",
                                    ObservationText = string.Empty,
                                    ObservationTag = ObservationTag.BloodTaken.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 15: //Urine Taken
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(15),
                                    ObservationCodeTerm = "Urine Sample Taken",
                                    ObservationText = string.Empty,
                                    ObservationTag = ObservationTag.UrineTaken.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 16: //BMI
                                if (string.IsNullOrWhiteSpace(fieldValue) || string.IsNullOrWhiteSpace(csv.GetField<string>(17))) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(16),
                                    ObservationCodeTerm = "Body Mass Index",
                                    ObservationValue = csv.GetField<decimal>(17),
                                    ObservationTag = ObservationTag.BMI.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 18: //Blood Pressure
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(18),
                                    ObservationCodeTerm = csv.GetField<string >(19),
                                    ObservationValue = string.IsNullOrEmpty(csv.GetField(20)) ? 0 : csv.GetField<decimal>(20),
                                    ObservationValue2 = string.IsNullOrEmpty(csv.GetField(21)) ? 0 : csv.GetField<decimal>(21),
                                    ObservationTag = ObservationTag.BloodPressure.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 22: //Cholesterol
                                if (string.IsNullOrWhiteSpace(fieldValue) || string.IsNullOrWhiteSpace(csv.GetField<string>(24))) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(22),
                                    ObservationCodeTerm = csv.GetField<string>(23),
                                    ObservationValue = csv.GetField<decimal>(24),
                                    ObservationTag = ObservationTag.Cholesterol.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 25: //Smoking
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(25),
                                    ObservationCodeTerm = csv.GetField<string>(26),
                                    ObservationTag = ObservationTag.SmokingStatus.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 27: //SmokingCessation
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(27),
                                    ObservationCodeTerm = csv.GetField<string>(28),
                                    ObservationTag = ObservationTag.SmokingCessation.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 29: //LLT Not Appropriate
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(29),
                                    ObservationCodeTerm = csv.GetField<string>(30),
                                    ObservationTag = ObservationTag.LLTNotAppropriate.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 31: //eGFR
                                if (string.IsNullOrWhiteSpace(fieldValue) || string.IsNullOrWhiteSpace(csv.GetField<string>(32))) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(31),
                                    ObservationCodeTerm = "eGFR",
                                    ObservationValue = csv.GetField<decimal>(32),
                                    ObservationTag = ObservationTag.eGFR.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 33: //ACR
                                if (string.IsNullOrWhiteSpace(fieldValue)|| string.IsNullOrWhiteSpace(csv.GetField<string>(34))) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(33),
                                    ObservationCodeTerm = "ACR",
                                    ObservationValue = csv.GetField<decimal>(34),
                                    ObservationTag = ObservationTag.ACR.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 35: //Activity
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(35),
                                    ObservationCodeTerm = csv.GetField<string>(36),
                                    ObservationTag = ObservationTag.Activity.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 37: //Exercise Education
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(37),
                                    ObservationCodeTerm = "Exercise Education Offered",
                                    ObservationTag = ObservationTag.ExerciseEducation.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 38: //Diet
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(38),
                                    ObservationCodeTerm = csv.GetField<string>(39),
                                    ObservationTag = ObservationTag.Diet.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 40: //Alcohol
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(40),
                                    ObservationCodeTerm = csv.GetField<string>(41),
                                    ObservationTag = ObservationTag.Alcohol.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 42: //Hba1C
                                if (string.IsNullOrWhiteSpace(fieldValue) || string.IsNullOrWhiteSpace(csv.GetField<string>(43))) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(42),
                                    ObservationValue = csv.GetField<decimal>(43),
                                    ObservationCodeTerm = "Hba1C",
                                    ObservationTag = ObservationTag.Hba1C.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 44: //Social Prescriber
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(44),
                                    ObservationCodeTerm = csv.GetField<string>(45),
                                    ObservationTag = ObservationTag.SocialPrescriber.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 46: //Weight Management
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(46),
                                    ObservationCodeTerm = csv.GetField<string>(47),
                                    ObservationTag = ObservationTag.WeightManagement.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 48: //Health Coach
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(48),
                                    ObservationCodeTerm = "Referred to Health Coach",
                                    ObservationTag = ObservationTag.HealthCoach.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 49: //FollowUps
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(49),
                                    ObservationCodeTerm = csv.GetField<string>(50),
                                    ObservationTag = ObservationTag.Followups.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 51: //Cholesterol
                                if (string.IsNullOrWhiteSpace(fieldValue) || string.IsNullOrWhiteSpace(csv.GetField<string>(53))) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(51),
                                    ObservationCodeTerm = csv.GetField<string>(52),
                                    ObservationValue = csv.GetField<decimal>(53),
                                    ObservationTag = ObservationTag.Cholesterol.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 54: //NewRx
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(54),
                                    ObservationCodeTerm = "Started On New Medication",
                                    ObservationText = csv.GetField<string>(55),
                                    ObservationTag = ObservationTag.NewRx.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 56: //Rx Exceptions
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(56),
                                    ObservationCodeTerm = csv.GetField<string>(57),
                                    ObservationTag = ObservationTag.RxExceptions.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 58: //On Max Tolerated Medication
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(58),
                                    ObservationCodeTerm = csv.GetField<string>(59),
                                    ObservationTag = ObservationTag.OnMaxToleratedRx.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 60: //Functional Assessment
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(60),
                                    ObservationCodeTerm = "Functional Assessment Score",
                                    ObservationValue = csv.GetField<decimal>(61),
                                    ObservationTag = ObservationTag.FunctionalAssessment.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 62: //Memory
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(62),
                                    ObservationCodeTerm = csv.GetField<string>(63),
                                    ObservationTag = ObservationTag.Memory.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 64: //CKD
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(64),
                                    ObservationCodeTerm = csv.GetField<string>(65),
                                    ObservationTag = ObservationTag.CKD.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 66: //FriendsFamily
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(66),
                                    ObservationText = csv.GetField<string>(67),
                                    ObservationTag = ObservationTag.FriendsFamily.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 68: //HealthConfidenceScore
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(68),
                                    ObservationValue = csv.GetField<decimal>(69),
                                    ObservationTag = ObservationTag.HealthConfidenceScore.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 71: //Antihypertensives
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(71),
                                    ObservationCodeTerm = csv.GetField<string>(70),
                                    ObservationTag = ObservationTag.Antihypertensives.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 73: //Lipid Lowering
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(73),
                                    ObservationCodeTerm = csv.GetField<string>(72),
                                    ObservationTag = ObservationTag.LipidLowering.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                            case 75: //SGLT2
                                if (string.IsNullOrWhiteSpace(fieldValue)) break;
                                observation = new ModelObservation
                                {
                                    ObservationID = Guid.NewGuid().ToString(),
                                    ObservationDate = csv.GetField<DateTime>(75),
                                    ObservationCodeTerm = csv.GetField<string>(74),
                                    ObservationTag = ObservationTag.SGLT2.ToString(),
                                    AppointmentID = appointmentID,
                                    PatientID = returnPatientID(csv.GetField<string>(0)),
                                };
                                observation.ObservationCheckSum =
                                    $"{observation.ObservationDate}{observation.PatientID}{observation.ObservationTag}";
                                if (!_checksums.Contains(observation.ObservationCheckSum) && observation.PatientID != String.Empty)
                                {
                                    observations.Add(observation);
                                    _checksums.Add(observation.ObservationCheckSum);
                                }
                                break;
                        }
                    }
                }
            }



            return observations;
        }

        private string returnPatientID(string NHSNumber)
        {
            NHSNumber = NHSNumber.Replace(" ", string.Empty);
            if (Repo.Instance.PatientIDsNHSNumber.ContainsValue(NHSNumber))
                    return Repo.Instance.PatientIDsNHSNumber.FirstOrDefault(x => x.Value == NHSNumber).Key.ToString();
            return string.Empty;
        }
    }
}
