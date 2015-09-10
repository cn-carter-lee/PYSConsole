using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PYS.Lib
{
    public class FtpFile
    {
        public string FileType { get; set; }

        public bool IsDirectory { get; set; }

        public DateTime LastWriteTime { get; set; }

        public long Length { get; set; }

        public string Name { get; set; }
    }
}
