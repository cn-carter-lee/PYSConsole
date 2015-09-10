using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinSCP;
using System.Data;
using System.Text.RegularExpressions;

namespace PYS.Lib
{
    public class WinScpFtpServer
    {
        public String Server { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String Path { get; set; }
        public String Filename { get; set; }
        public String WinscpPath { get; set; }
        public String SshHostKeyFingerprint { get; set; }
        public String FilterPattern { get; set; }
        public String SortOrder { get; set; }

        public DataTable Load()
        {
            StringBuilder requestUri = new StringBuilder();
            SessionOptions sessionOptions = new SessionOptions
            {
                HostName = Server,
                UserName = Username,
                Password = Password,
            };

            sessionOptions.Protocol = WinSCP.Protocol.Ftp;
            if (!String.IsNullOrEmpty(Path))
            {
                if (!Path.StartsWith("/") && !Path.StartsWith("."))
                {
                    requestUri.Append("/");
                }
                requestUri.Append(Path);
            }
            // sessionOptions.SshHostKeyFingerprint = SshHostKeyFingerprint;

            if (!String.IsNullOrEmpty(Path))
            {
                if (!Path.StartsWith("/") && !Path.StartsWith("."))
                {
                    requestUri.Append("/");
                }
                requestUri.Append(Path);
            }


            using (Session session = new Session())
            {
                if (!String.IsNullOrEmpty(WinscpPath))
                {
                    session.ExecutablePath = WinscpPath;
                }

                // prepare for output
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Name", typeof(String));
                dataTable.Columns.Add("Size", typeof(long));
                dataTable.Columns.Add("LastModifiedDate", typeof(DateTime));
                dataTable.Columns.Add("FullPath", typeof(String));

                session.Open(sessionOptions);
                RemoteDirectoryInfo dirListing = session.ListDirectory(Path);
                Regex regex = null;
                if (!String.IsNullOrEmpty(FilterPattern))
                {
                    regex = new Regex(FilterPattern);
                }
                foreach (RemoteFileInfo fileInfo in dirListing.Files)
                {
                    if (regex == null || regex.IsMatch(fileInfo.Name))
                    {
                        dataTable.Rows.Add(new object[] { fileInfo.Name, fileInfo.Length, fileInfo.LastWriteTime, String.Format("{0}/{1}", requestUri.ToString(), fileInfo.Name) });
                    }
                }


                if (dataTable.Rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(SortOrder))
                    {
                        if (String.Compare(SortOrder, "asc", true) == 0)
                        {
                            DataView defaultView = dataTable.DefaultView;
                            defaultView.Sort = "LastModifiedDate ASC";
                            dataTable = defaultView.ToTable();
                        } if (String.Compare(SortOrder, "desc", true) == 0)
                        {
                            DataView defaultView = dataTable.DefaultView;
                            defaultView.Sort = "LastModifiedDate DESC";
                            dataTable = defaultView.ToTable();
                        }
                    }
                }
                return dataTable;
            }
        }
                              
        public static void Test()
        {
            WinScpFtpServer sftpServer = new WinScpFtpServer()
            {
                Server = "208.15.91.24",
                Username = "wcuser",
                Password = "8N75559f",
                WinscpPath = @"C:\Program Files (x86)\WinSCP\WinSCP.exe",
                Path = "/users/wcuser/wchannel/"
            };
            DataTable table = sftpServer.Load();
            foreach (DataRow row in table.Rows)
            {
                Console.WriteLine(".........................................................\n");
                Console.WriteLine("Name : {0}", row[0].ToString());
                Console.WriteLine("Size : {0}", row[1].ToString());
                Console.WriteLine("LastModifiedDate : {0}", row[2].ToString());
                Console.WriteLine("FullPath {0}", row[3].ToString());
            }
        }
    }
}
