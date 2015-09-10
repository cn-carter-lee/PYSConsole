using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using Microsoft.Win32;

namespace PYS.Utility
{
    public class PysCommon
    {
        public static void ShowSystemValue()
        {
            string processor = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            Console.WriteLine("PROCESSOR_ARCHITECTURE : {0}", processor);
            Console.WriteLine("Complete...");
            Console.ReadLine();
        }

        public static void LoadDiaperFiles()
        {
            SqlConnection conn = new SqlConnection("server=localhost;Initial Catalog=CarterTemp;Integrated Security=True;");
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
                DirectoryInfo dir = new DirectoryInfo(@"D:\Diaper\");
                FileInfo[] listFileInfo = dir.GetFiles();
                for (int i = 0; i < listFileInfo.Length; i++)
                {
                    comm.CommandText = string.Format(@"
                    insert into RetailerRegistryHistory(
                    FileName,
                    RetailerId,
                    RetailerRegistryCode,                              
                    EventDate,                              
                    RegistrantLastName,                            
                    RegistrantFirstName,                             
                    CoRegistrantLastName,                            
                    CoRegistrantFirstName,                           
                    RegistrantEmail,                           
                    City,
                    State,                            
                    Zip,                            
                    CoRegistrantEmail,                                                                                     
                    IsReferred,                               
                    WcSid,                                
                    NotSearchable )      
                    select
                    '{0}',   
                    14670,
                    RetailerRegistryCode,                              
                    EventDate,                              
                    RegistrantLastName,                            
                    RegistrantFirstName,                             
                    CoRegistrantLastName,                            
                    CoRegistrantFirstName,                           
                    RegistrantEmail,                           
                    RegistrantCity,                           
                    RegistrantState,                            
                    RegistrantZip,                            
                    CoRegistrantEmail,                              
                    IsReferred,                               
                    WcSid,                                
                    NotSearchable                                     
                    from openrowset( bulk  'D:\Diaper\{1}',
                    formatfile = 'D:\SSISPackages\Registry\cli\Header\HeaderFormatFiles\Diapers.fmt',errorfile = 'D:\SSISPackages\Registry\cli\Header\HeaderErrorFiles\DIAPERS_errorfile.txt',
                    firstrow = 1,rows_per_batch =1000) as b", listFileInfo[i].Name, listFileInfo[i].Name);
                    comm.ExecuteNonQuery();
                }
            }
            catch { }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        static void DBTransaction()
        {
            SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=Northwind;Integrated Security=SSPI;");
            connection.Open();
            // Create a new transaction
            SqlTransaction transaction = connection.BeginTransaction();
            // Create a command for transaction
            SqlCommand myCommand = new SqlCommand();
            myCommand.Connection = connection;
            myCommand.Transaction = transaction;
            try
            {
                myCommand.CommandText = "Insert into Region (RegionID, RegionDescription) VALUES (100, 'Description')";
                myCommand.ExecuteNonQuery();
                myCommand.CommandText = "Insert into Region (RegionID, RegionDescription) VALUES (101, 'Description')";
                myCommand.ExecuteNonQuery();
                transaction.Commit();
                Console.WriteLine("Both records are written to database.");
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Console.WriteLine(e.ToString());
                Console.WriteLine("Neither record was written to database.");
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Change file's Modified Date
        /// </summary>
        public static void ChangeFileModifiedDate()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(@"D:\a");
            foreach (var file in directoryInfo.GetFiles())
            {
                file.LastWriteTime = new DateTime(2013, 8, 16, 0, 0, 0);
            }
        }

        public static void ReadText()
        {
            using (StreamReader sr = new StreamReader(@"D:\federated_incremental.txt"))
            {
                string line = string.Empty;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    Console.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public static string CalculateMd5CheckSum(string fileLocation)
        {
            System.Security.Cryptography.HashAlgorithm hmacMd5 = new System.Security.Cryptography.HMACMD5();
            byte[] hashByte;
            using (Stream fileStream = new FileStream(fileLocation, FileMode.Open))
            using (Stream bufferedStream = new BufferedStream(fileStream, 1200000))
                hashByte = hmacMd5.ComputeHash(bufferedStream);
            StringBuilder sbResult = new StringBuilder();
            for (int i = 0; i < hashByte.Length; i++)
            {
                sbResult.Append(hashByte[i].ToString("x2"));
            }
            return sbResult.ToString().ToUpper();
        }
    }
}
