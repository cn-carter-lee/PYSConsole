using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Net;
using System.Collections.Specialized;

public class TestExcel
{
    public static void Run()
    {
        NameValueCollection nvc = new NameValueCollection();
        nvc.Add("id", "TTR");
        nvc.Add("btn-submit-photo", "Upload");
        HttpUploadFile("http://dev.carter.com/api/Product/Create",
             @"D:\test.jpg", "file", "image/jpeg", nvc);

    }

    public static void HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
    {
        // log.Debug(string.Format("Uploading {0} to {1}", file, url));
        string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
        byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

        HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
        wr.ContentType = "multipart/form-data; boundary=" + boundary;
        wr.Method = "POST";
        wr.KeepAlive = true;
        wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

        Stream rs = wr.GetRequestStream();

        string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
        foreach (string key in nvc.Keys)
        {
            rs.Write(boundarybytes, 0, boundarybytes.Length);
            string formitem = string.Format(formdataTemplate, key, nvc[key]);
            byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
            rs.Write(formitembytes, 0, formitembytes.Length);
        }
        rs.Write(boundarybytes, 0, boundarybytes.Length);

        string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
        string header = string.Format(headerTemplate, paramName, file, contentType);
        byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
        rs.Write(headerbytes, 0, headerbytes.Length);

        FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
        byte[] buffer = new byte[4096];
        int bytesRead = 0;
        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        {
            rs.Write(buffer, 0, bytesRead);
        }
        fileStream.Close();

        byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
        rs.Write(trailer, 0, trailer.Length);
        rs.Close();

        WebResponse wresp = null;
        try
        {
            wresp = wr.GetResponse();
            Stream stream2 = wresp.GetResponseStream();
            StreamReader reader2 = new StreamReader(stream2);
            // log.Debug(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
        }
        catch (Exception ex)
        {
            // log.Error("Error uploading file", ex);
            if (wresp != null)
            {
                wresp.Close();
                wresp = null;
            }
        }
        finally
        {
            wr = null;
        }
    }


    public static void ReadExcel()
    {
        string sheetName = string.Empty;
        string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\test.xlsx;Extended Properties=\"Excel 12.0;HDR=No;IMEX=1\"";
        OleDbConnection connection = new OleDbConnection(connectionString);
        connection.Open();
        DataTable table = connection.GetSchema("Tables");

        if (table.Rows.Count > 0)
            sheetName = table.Rows[0]["TABLE_NAME"].ToString();

        if (!string.IsNullOrEmpty(sheetName))
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = string.Format("select * from [{0}]", sheetName);
            command.CommandType = CommandType.Text;
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                StringBuilder strBuilder = new StringBuilder();
                for (int i = 0; i < reader.FieldCount; i++)
                    strBuilder.Append(string.Format("{0}\t", reader[i]));
                Console.WriteLine(strBuilder.ToString());
            }
            reader.Close();
        }

        connection.Close();

        Console.ReadKey();
    }
}
