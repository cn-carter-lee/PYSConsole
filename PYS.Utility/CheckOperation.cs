using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace PYS.Utility
{
    public static class CheckOperation
    {
        public static void CheckRetailerData(int retalerid)
        {
            StringBuilder result = new StringBuilder();
            SqlConnection conn = new SqlConnection("Data Source=PRDMSSQLCLSDB01\\DW;Initial Catalog=RegistryOps;Integrated Security=True;");
            try
            {
                string commandText = string.Format("declare @RetailerId smallint = {0}     select retailer_registry_id from CoreRegistryRptData.[DELLAJAMES].[RETAILER_REGISTRY] a  where a.Retailer_UID = @RetailerId and not exists(select 1 from Header.RetailerRegistryHistory b where a.Retailer_Registry_id = b.RetailerRegistryCode and b.RetailerId = @RetailerId) and ACTIVE_FLAG <> 'D'", retalerid);
                SqlCommand comm = new SqlCommand(commandText, conn);
                SqlDataAdapter adp = new SqlDataAdapter(commandText, conn);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (conn.State == ConnectionState.Closed) conn.Open();
                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    SqlCommand comm2 = new SqlCommand();
                    foreach (DataRow row in dt.Rows)
                    {
                        string commandText2 = string.Format("SELECT Message FROM [header].[LoadingRegistryLog] WHERE RowData like '%{0}%'", row["retailer_registry_id"].ToString());
                        comm2.CommandText = commandText2;
                        comm2.Connection = conn;
                        object obj = comm2.ExecuteScalar();
                        if (obj != null)
                        {
                            result.AppendLine(string.Format("{0}------{1}", row["retailer_registry_id"].ToString(), obj.ToString()));
                        }
                        else
                        {
                            result.AppendLine(string.Format("{0}------{1}", row["retailer_registry_id"].ToString(), ""));
                        }
                    }
                }
            }
            catch
            {

            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            string filename = string.Format("D:\\{0}.txt", retalerid);
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            file.WriteLine(result.ToString());
            file.Close();
        }

    }
}
