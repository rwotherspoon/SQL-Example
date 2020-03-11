using System;
using System.IO;
using System.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;

namespace AccessTracker
{
    class Program
    {
        static void Main()
        {
            string CS = @"Data Source = (localdb)\ProjectsV13; Initial Catalog = AccessTracker; Integrated Security = True";

            CreateDataBase(CS);
            Console.WriteLine("Access Tracker Database Created!");
            Console.WriteLine("");

            LoadActiveWorkers(CS);
            Console.WriteLine("Active Worker Records Loaded!");
            Console.WriteLine("");

            LoadAccessRecords(CS);
            Console.WriteLine("Access Records Loaded!");
            Console.WriteLine("");

            //LoadTrainingRecords(CS);
            Console.WriteLine("Training Records Loaded!");
            Console.WriteLine("");
            Console.WriteLine("Data Import Complete!");

            LoadCompanyType(CS);
            Console.WriteLine("");
            Console.WriteLine("Company Type Populated!");

            LoadWorkerAccessType(CS);
            Console.WriteLine("");
            Console.WriteLine("Access Type Populated!");

            LoadDukeWorkerCity(CS);
            Console.WriteLine("");
            Console.WriteLine("City Populated for Duke Workers!");

            LoadWorkerRegion(CS);
            Console.WriteLine("");
            Console.WriteLine("Region Populated for ALL Workers!");

            Console.WriteLine("");
            Console.WriteLine("Database Population 100% Complete!");

            Console.ReadLine();
        }

        private static void CreateDataBase(string s)
        {
            string qryText = File.ReadAllText("AccessTracker DB Creation Script.sql");
            using (SqlConnection c = new SqlConnection(s))
            {
                c.Open();
                using (SqlCommand cmd = new SqlCommand(qryText, c))
                {
                    cmd.Parameters.Clear();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void LoadActiveWorkers(string s)
        {
            using (SqlConnection c = new SqlConnection(s))
            {
                c.Open();
                string cmdString = "INSERT INTO tblActiveWorkers (WorkerID, WorkerFullName, WorkerFirstName, ";
                       cmdString += "WorkerLastName, ManagerFullName, WorkerCity, WorkerState, VendorName) ";
                       cmdString += "VALUES(@WorkerID, @WorkerFullName, @WorkerFName, @WorkerLName, @MGRFullName,";
                       cmdString += "@WorkerCity, @WorkerState, @VendorName)";

                using (SqlCommand cmd = new SqlCommand(cmdString, c))
                {
                    string filename = "ActiveWorkers.csv";
                    TextFieldParser p = Parser(filename);
                    int count = 0;

                    while (!p.EndOfData)
                    {
                        cmd.Parameters.Clear();
                        string[] fields = p.ReadFields();
                        cmd.Parameters.Add(new SqlParameter("@WorkerID", fields[0].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@WorkerFullName", fields[1].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@WorkerFName", fields[2].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@WorkerLName", fields[3].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@MGRFullName", fields[44].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@WorkerCity", fields[52].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@WorkerState", fields[53].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@VendorName", fields[30].ToString()));
                        cmd.ExecuteNonQuery();
                        count++;
                    }
                    Console.WriteLine("Total Worker Records: " + count);
                }                                   
            }
        }

        private static void LoadAccessRecords(string s)
        {
            using (SqlConnection c = new SqlConnection(s))
            {
                c.Open();
                string cmdString = "INSERT INTO tblAccess (WorkerID, WorkerLastName, WorkerFirstName, WorkerFullName, BusinessRole, CompanyName, ManagerFullName)";
                cmdString += " VALUES(@WorkerID, @WorkerLName, @WorkerFName, @WorkerFullName, @BusinessRole, @CompanyName, @MGRFullName)";
                using (SqlCommand cmd = new SqlCommand(cmdString, c))
                {
                    string filename = "AccessRecords.csv";
                    TextFieldParser p = Parser(filename);
                    int count = 0;

                    while (!p.EndOfData)
                    {
                        cmd.Parameters.Clear();
                        string[] fields = p.ReadFields();
                        cmd.Parameters.Add(new SqlParameter("@WorkerID", fields[0].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@WorkerLName", fields[1].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@WorkerFName", fields[2].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@WorkerFullName", fields[3].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@BusinessRole", fields[4].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@CompanyName", fields[5].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@MGRFullName", fields[6].ToString()));
                        cmd.ExecuteNonQuery();
                        count++;
                    }
                    Console.WriteLine("Total Access Records: " + count);
                }
            }                
        }

        private static void LoadTrainingRecords(string s)
        {
            using (SqlConnection c = new SqlConnection(s))
            {
                c.Open();
                string cmdString = "INSERT INTO tblTraining (WorkerID, TrainingExpirationDate, WorkerFullName, ManagerFullName, WorkerDepartment, TrainingClass, BusinessRole, ComplianceMonitor)";
                cmdString += " VALUES(@WorkerID, @ExpirationDate, @WorkerName, @ManagerName, @DepartmentName, @Training, @BusinessRole, @ComplianceMonitor)";
                using (SqlCommand cmd = new SqlCommand(cmdString, c))
                {
                    string filename1 = "TrainingRecords.csv";
                    TextFieldParser p1 = Parser(filename1);
                    int count = 0;

                        while (!p1.EndOfData)
                        {
                            cmd.Parameters.Clear();
                            string[] fields = p1.ReadFields();
                            cmd.Parameters.Add(new SqlParameter("@WorkerID", fields[0].ToString()));
                            cmd.Parameters.Add(new SqlParameter("@ExpirationDate", fields[1].ToString()));
                            cmd.Parameters.Add(new SqlParameter("@WorkerName", fields[2].ToString()));
                            cmd.Parameters.Add(new SqlParameter("@ManagerName", fields[5].ToString()));
                            cmd.Parameters.Add(new SqlParameter("@DepartmentName", fields[6].ToString()));
                            cmd.Parameters.Add(new SqlParameter("@Training", fields[7].ToString()));
                            cmd.Parameters.Add(new SqlParameter("@BusinessRole", fields[8].ToString()));
                            cmd.Parameters.Add(new SqlParameter("@ComplianceMonitor", fields[9].ToString()));
                            cmd.ExecuteNonQuery();
                            count++;
                        }
                    Console.WriteLine("Total Training Records: " + count);
                }                    
            }
        }

        private static void LoadCompanyType(string s)
        {
            using (SqlConnection c = new SqlConnection(s))
            {
                c.Open();
                string cmdString = "update tblAccess set WorkerCompanyType = ";
                       cmdString += "(case when dbo.tblAccess.CompanyName like 'duke%' then 'Duke'";
                       cmdString += "when dbo.tblAccess.CompanyName like 'jones%' then 'JLL'";
                       cmdString += "else 'Vendor' end)";
                using (SqlCommand cmd = new SqlCommand(cmdString, c))
                {
                    cmd.Parameters.Clear();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void LoadWorkerAccessType(string s)
        {
            using (SqlConnection c = new SqlConnection(s))
            {
                c.Open();
                string cmdString = "update tblAccess set WorkerAccessType = ";
                       cmdString += "(case when dbo.tblAccess.BusinessRole like 'aco%' then 'Physical'";
                       cmdString += "when dbo.tblAccess.BusinessRole like '%field%' then 'Electronic'";
                       cmdString += "when dbo.tblAccess.BusinessRole like '%bes csi%' then 'BES CSI'";
                       cmdString += "when dbo.tblAccess.BusinessRole like 'enterprise%' then 'NCIDM'";
                       cmdString += "else 'N/A' end)";
                using (SqlCommand cmd = new SqlCommand(cmdString, c))
                {
                    cmd.Parameters.Clear();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void LoadDukeWorkerCity(string s)
        {
            using (SqlConnection c = new SqlConnection(s))
            {
                c.Open();
                string cmdString = "update tblAccess set WorkerCity = ";
                       cmdString += "tblActiveWorkers.WorkerCity ";
                       cmdString += "from tblActiveWorkers ";
                       cmdString += "where tblAccess.WorkerID = tblActiveWorkers.WorkerID ";
                       cmdString += "and tblAccess.WorkerCompanyType = 'duke'";
                using (SqlCommand cmd = new SqlCommand(cmdString, c))
                {
                    cmd.Parameters.Clear();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void LoadWorkerRegion(string s)
        {
            using (SqlConnection c = new SqlConnection(s))
            {
                c.Open();
                string cmdString1 = "update tblAccess set WorkerRegion = tblCityRegion.Region ";
                       cmdString1 += "from tblCityRegion ";
                       cmdString1 += "where tblAccess.WorkerCity = tblCityRegion.City";

                string cmdString2 = "update tblAccess set WorkerRegion = tblMgrRegion.Region ";
                       cmdString2 += "from tblMgrRegion ";
                       cmdString2 += "where tblAccess.ManagerFullName = tblMgrRegion.MgrName ";
                       cmdString2 += "and tblAccess.WorkerCompanyType != 'duke'";

                using (SqlCommand cmd = new SqlCommand(cmdString1, c))
                {
                    cmd.Parameters.Clear();
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(cmdString2, c))
                {
                    cmd.Parameters.Clear();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static TextFieldParser Parser(string filename)
        {
            TextFieldParser p = new TextFieldParser(filename);
            p.TextFieldType = FieldType.Delimited;
            p.SetDelimiters(",");
            p.HasFieldsEnclosedInQuotes = true;
            p.ReadLine();
            return p;
        }
    }
}