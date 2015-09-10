using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace PYS.Lib
{
    // <summary>  
    /// Global Assembly Cache manager  
    /// </summary>  
    public sealed class GlobalAssemblyCache : ICollection<AssemblyName>
    {
        private readonly Process m_gacutil;
        //Path to the GACUTIL tool  
        private const string GacUtilExe = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\x64\gacutil.exe ";

        public GlobalAssemblyCache()
        {
            m_gacutil = new Process();
            m_gacutil.StartInfo.FileName = GacUtilExe;
            m_gacutil.StartInfo.RedirectStandardOutput = true;
            m_gacutil.StartInfo.UseShellExecute = false;
        }

        #region ICollection<AssemblyName> Members

        /// <summary>  
        /// Add a new assembly to the GAC  
        /// </summary>  
        /// <param name="item">Display name of the assembly</param>  
        public void Add(AssemblyName item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            m_gacutil.StartInfo.Arguments = String.Format("/i \"{0}\"", item.CodeBase);
            m_gacutil.Start();
        }

        /// <summary>  
        /// Delete all assemblies from GAC  
        /// </summary>  
        void ICollection<AssemblyName>.Clear()
        {
            foreach (var item in this)
                Remove(item);
        }

        /// <summary>  
        /// Returns value indicating that assembly with specified name is already installed in the GAC  
        /// </summary>  
        /// <param name="item">Assembly name</param>  
        /// <returns>true, if assembly is installed, otherwise, false</returns>  
        public bool Contains(AssemblyName item)
        {
            if (item == null) return false;
            var queryResult = from asm in this
                              where AssemblyName.ReferenceMatchesDefinition(item, asm)
                              select true;
            return queryResult.FirstOrDefault();
        }

        void ICollection<AssemblyName>.CopyTo(AssemblyName[] array, int arrayIndex)
        {
            this.ToArray().CopyTo(array, arrayIndex);
        }

        /// <summary>  
        /// Get count of assemblies in the GAC  
        /// </summary>  
        public int Count
        {
            get { return this.Count(); }
        }


        bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>  
        /// Remove assembly from GAC  
        /// </summary>  
        /// <param name="item"></param>  
        /// <returns></returns>  
        public bool Remove(AssemblyName item)
        {
            if (item == null) return false;
            m_gacutil.StartInfo.Arguments = String.Format("/u \"{0}\"", item);
            return m_gacutil.Start();
        }

        #endregion

        #region IEnumerable<AssemblyName> Members

        /// <summary>  
        /// Get GAC iterator  
        /// </summary>  
        /// <returns></returns>  
        public IEnumerator<AssemblyName> GetEnumerator()
        {
            m_gacutil.StartInfo.Arguments = "/l";
            m_gacutil.Start();
            var output = m_gacutil.StandardOutput;
            while (!output.EndOfStream)
                yield return CreateAssemblyNameSafe(output.ReadLine());
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private AssemblyName CreateAssemblyNameSafe(string assemblyName)
        {
            var result = default(AssemblyName);
            try
            {
                result = new AssemblyName(assemblyName);
            }
            catch
            {
                result = null;
            }
            return result;
        }

        bool ICollection<AssemblyName>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public static void Test(string writePath)
        {
            StringBuilder result = new StringBuilder();
            GlobalAssemblyCache gac = new GlobalAssemblyCache();
            foreach (var item in gac)
            {
                if (item != null)
                    //    if (item.FullName.IndexOf("XO.Registry") > -1)
                    result.AppendLine(item.FullName);
            }
            string filename = string.Format(writePath);
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            file.WriteLine(result.ToString());
            Console.WriteLine("Complete!");
        }
    }
}
