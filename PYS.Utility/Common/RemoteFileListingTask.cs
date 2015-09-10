using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WinSCP;
using System.Text.RegularExpressions;

public class RemoteFileLisTask
{
    public String Protocol { get; set; }
    public String Server { get; set; }
    public String Username { get; set; }
    public String Password { get; set; }
    public String Path { get; set; }
    public String Filename { get; set; }
    public String WinscpPath { get; set; }
    public String SshHostKeyFingerprint { get; set; }
    public String FilterPattern { get; set; }
    public String SortOrder { get; set; }

    public DataSet Files;

    public void Execute()
    {
        StringBuilder requestUri = new StringBuilder();
        SessionOptions sessionOptions = new SessionOptions
        {
            HostName = Server,
            UserName = Username,
            Password = Password,
        };
        if (Protocol.ToLower().CompareTo("ftp") == 0)
        {
            sessionOptions.Protocol = WinSCP.Protocol.Ftp;

            if (!String.IsNullOrEmpty(Path))
            {
                if (!Path.StartsWith("/") && !Path.StartsWith("."))
                {
                    requestUri.Append("/");
                }
                requestUri.Append(Path);
            }
        }
        else if (Protocol.ToLower().CompareTo("sftp") == 0)
        {
            sessionOptions.Protocol = WinSCP.Protocol.Sftp;
            sessionOptions.SshHostKeyFingerprint = SshHostKeyFingerprint;

            if (!String.IsNullOrEmpty(Path))
            {
                if (!Path.StartsWith("/") && !Path.StartsWith("."))
                {
                    requestUri.Append("/");
                }
                requestUri.Append(Path);
            }
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
            dataTable.Columns.Add("Size", typeof(int));
            dataTable.Columns.Add("LastModifiedDate", typeof(DateTime));
            dataTable.Columns.Add("FullPath", typeof(String));

            session.Open(sessionOptions);
            RemoteDirectoryInfo dirListing = session.ListDirectory(requestUri.ToString());
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

            Files = new DataSet();
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
                Files.Tables.Add(dataTable);
            }
        }
    }

    public static void Test()
    {
        while (true)
        {
            Console.WriteLine(".............................................................");
            Console.WriteLine("StartTime : {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            RemoteFileLisTask remoteFileListingTask = new RemoteFileLisTask()
            {
                Protocol = "ftp",
                Server = "datatransfer.cj.com",
                Username = "2667603",
                Password = "YF4Xm-kt",
                Path = "/outgoing",
                Filename = "",
                WinscpPath = @"C:\Program Files (x86)\WinSCP\WinSCP.exe",
                SshHostKeyFingerprint = "",
                FilterPattern = "2667603_126565_",
                SortOrder = "desc",
            };

            remoteFileListingTask.Execute();
            if (remoteFileListingTask.Files.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in remoteFileListingTask.Files.Tables[0].Rows)
                {
                    Console.WriteLine("{0}", row[0].ToString());
                }

            }
        }
    }
}

