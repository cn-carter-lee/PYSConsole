using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.FtpClient;

namespace PYS.Lib
{
    /// <summary>
    /// Remark: Based on WinSCP
    /// </summary>
    public class FtpServer
    {
        public bool IsSFTP { get; set; }

        public string HostName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string SshHostKeyFingerprint { get; set; }

        public string Path { get; set; }

        public FtpServer()
        {

        }

        public FtpServer(bool isSFTP, string hostName, string userName, string password)
        {
            this.IsSFTP = isSFTP;
            this.HostName = hostName;
            this.UserName = userName;
            this.Password = password;
        }

        public FtpServer(bool isSFTP, string hostName, string userName, string password, string sshHostKeyFingerprint)
            : this(isSFTP, hostName, userName, password)
        {
            this.SshHostKeyFingerprint = sshHostKeyFingerprint;
        }

        public List<FtpFile> GetListFiles()
        {
            List<FtpFile> listFtpFiles = new List<FtpFile>();

            StringBuilder requestUri = new StringBuilder();
            if (string.IsNullOrEmpty(this.Path))
            {
                requestUri.Append("/");
            }
            else
            {
                if (!this.Password.StartsWith("/"))
                {
                    requestUri.Append("/");
                }
                requestUri.Append(Path);
                if (!Path.EndsWith("/"))
                {
                    requestUri.Append("'/");
                }
            }

            FtpClient ftpClient = new FtpClient(UserName, Password, HostName);
            
            FtpListItem[] listItems = ftpClient.GetListing(requestUri.ToString());
            foreach (FtpListItem item in listItems)
            {
                FtpFile ftpFile = new FtpFile()
                {
                    FileType = item.Type.ToString(),
                    Name = item.Name,
                    LastWriteTime = item.Modify
                };
                listFtpFiles.Add(ftpFile);
            }
            return listFtpFiles;
        }

        public static void Test()
        {
            List<FtpFile> listFtpFile = new List<FtpFile>();
            FtpServer ftpServer = new FtpServer()
            {
                HostName = "172.25.12.20",
                IsSFTP = false,
                UserName = "qa",
                Password = "abcd1234!",
                Path = "/KOHLS/"
            };
            listFtpFile = ftpServer.GetListFiles();
            foreach (FtpFile file in listFtpFile)
            {
                Console.WriteLine("{0}   {1}", file.Name, file.LastWriteTime.ToString());
            }
        }
    }
}
