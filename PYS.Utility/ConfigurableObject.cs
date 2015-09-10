using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XO.Registry.Utility
{
    public class ConfigurableObject
    {
        public IDictionary Config { get; private set; }

        public ConfigurableObject(IDictionary config)
        {
            Config = config;
        }

        public string this[string key]
        {
            get
            {
                return this.Config[key] as string;
            }
        }
    }
}
