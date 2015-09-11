using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PYS.Tutorial.Constuctor
{
    public class NLog
    {
        // 通过将构造函数设置为私有构造函数，可以阻止类被实例化
        // Private Constructor
        private NLog() { }

        public static double e = Math.E;
    }
}
